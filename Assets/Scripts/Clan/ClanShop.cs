using System;
using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using LuyenDan;
using Network;
using Network.Core;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;

namespace Clan
{
    /// <summary>Hiển thị kho tông môn và xử lý mua hoặc gỡ vật phẩm.</summary>
    public class ClanShop : ClanPopup
    {
        [Header("Danh sách")]
        [SerializeField] private Bag1ItemPrefabInventory itemList;
        [SerializeField] private GameObject emptyState;
        [SerializeField] private GameObject loadingState;

        [Header("Thông tin")]
        [SerializeField] private Text contributionPointsText;
        [SerializeField] private Text selectedItemText;
        [SerializeField] private Text priceText;
        [SerializeField] private Text totalPriceText;
        [SerializeField] private InputField quantityInput;

        [Header("Thao tác")]
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button removeButton;
        [SerializeField] private Button openBagButton;
        [SerializeField] private ClanBag clanBag;

        private readonly List<ClanShopItem> renderedItems = new();
        private readonly List<ItemRef> renderedItemRefs = new();
        private ClanShopItem selectedItem;
        private bool isLoading;
        private bool isSubmitting;

        public IReadOnlyList<ClanShopItem> Items =>
            Handler?.CurrentClan?.ShopItems ?? (IReadOnlyList<ClanShopItem>)Array.Empty<ClanShopItem>();
        public ClanShopItem SelectedItem => selectedItem;
        public event Action<ClanShopItem> SelectionChanged;

        public long ContributionPoints
        {
            get
            {
                var characterId = Client.Instance?.CharacterData?.Id ?? 0;
                return Handler?.CurrentClan?.Members?
                           .FirstOrDefault(member => member.CharId == characterId)?.ContribPoints ?? 0;
            }
        }

        protected override void Start()
        {
            base.Start();
            SetEventButton(refreshButton, Refresh);
            SetEventButton(buyButton, BuySelected);
            SetEventButton(removeButton, RemoveSelected);
            SetEventButton(openBagButton, OpenBag);
            if (quantityInput != null)
            {
                quantityInput.onValueChanged.RemoveListener(OnQuantityChanged);
                quantityInput.onValueChanged.AddListener(OnQuantityChanged);
            }
            UpdateSelectedView();
        }

        protected override void BindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged += OnClanChanged;
            handler.ShopChanged += OnShopChanged;
            handler.MemberPointsChanged += OnMemberPointsChanged;
            handler.ContributionChanged += OnContributionChanged;
        }

        protected override void UnbindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged -= OnClanChanged;
            handler.ShopChanged -= OnShopChanged;
            handler.MemberPointsChanged -= OnMemberPointsChanged;
            handler.ContributionChanged -= OnContributionChanged;
        }

        protected override void OnShown()
        {
            Render(Items);
            Refresh();
        }

        public void Select(ClanShopItem item)
        {
            selectedItem = item;
            SetQuantity(1);
            UpdateSelectedView();
            SelectionChanged?.Invoke(selectedItem);
        }

        public void Refresh()
        {
            if (!isLoading) RefreshAsync();
        }

        public void BuySelected()
        {
            if (!isSubmitting) BuySelectedAsync();
        }

        public void RemoveSelected()
        {
            if (!isSubmitting) RemoveSelectedAsync();
        }

        private async void RefreshAsync()
        {
            if (!InitializeHandler() || Handler.CurrentClan == null) return;
            isLoading = true;
            SetLoading(true);
            try
            {
                var result = await Handler.GetShop();
                if (!result.Success) Notify(result.Message);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                Notify("Không thể tải kho tông môn");
            }
            finally
            {
                isLoading = false;
                SetLoading(false);
            }
        }

        private async void BuySelectedAsync()
        {
            if (!CanBuy(out var quantity, out var message))
            {
                Notify(message);
                return;
            }

            isSubmitting = true;
            UpdateButtons();
            try
            {
                var result = await Handler.BuyShopItem(selectedItem.Id, quantity);
                Notify(result.Message);
                if (result.Success) UpdateContributionPoints();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                Notify("Không thể mua vật phẩm");
            }
            finally
            {
                isSubmitting = false;
                UpdateSelectedView();
            }
        }

        private async void RemoveSelectedAsync()
        {
            if (!CanRemove(out var message))
            {
                Notify(message);
                return;
            }

            var removedId = selectedItem.Id;
            isSubmitting = true;
            UpdateButtons();
            try
            {
                var result = await Handler.RemoveShopItem(removedId);
                Notify(result.Message);
                if (result.Success)
                {
                    selectedItem = null;
                    Render(Items);
                    SelectionChanged?.Invoke(null);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                Notify("Không thể gỡ vật phẩm khỏi kho tông môn");
            }
            finally
            {
                isSubmitting = false;
                UpdateSelectedView();
            }
        }

        private bool CanBuy(out int quantity, out string message)
        {
            quantity = ReadQuantity();
            message = string.Empty;
            if (Handler == null || Handler.CurrentClan == null) message = "Bạn chưa thuộc tông môn";
            else if (selectedItem == null) message = "Hãy chọn vật phẩm muốn mua";
            else if (Handler.IsShopItemOwner(selectedItem)) message = "Chủ sở hữu không thể mua vật phẩm của mình";
            else if (Handler.MaxShopQuantityPerRequest <= 0) message = "Kho tông môn đang tạm khóa giao dịch";
            else if (quantity < 1) message = "Số lượng mua phải lớn hơn 0";
            else if (selectedItem.Item == null || quantity > selectedItem.Item.Count) message = "Kho không còn đủ số lượng";
            else if (Handler.MaxShopQuantityPerRequest > 0 && quantity > Handler.MaxShopQuantityPerRequest)
                message = $"Mỗi lần chỉ được mua tối đa {Handler.MaxShopQuantityPerRequest}";
            else if (selectedItem.Price > 0 && quantity > long.MaxValue / selectedItem.Price)
                message = "Tổng giá trị giao dịch quá lớn";
            else if (ContributionPoints < selectedItem.Price * quantity) message = "Không đủ điểm cống hiến";
            return string.IsNullOrEmpty(message);
        }

        private bool CanRemove(out string message)
        {
            message = string.Empty;
            if (Handler == null || Handler.CurrentClan == null) message = "Bạn chưa thuộc tông môn";
            else if (selectedItem == null) message = "Hãy chọn vật phẩm muốn gỡ";
            else if (!Handler.IsShopItemOwner(selectedItem)) message = "Chỉ chủ sở hữu mới có thể gỡ vật phẩm này";
            else if (!HasPermission(ClanPermission.RemoveShopItem)) message = "Bạn không có quyền gỡ vật phẩm khỏi kho";
            return string.IsNullOrEmpty(message);
        }

        private void Render(IEnumerable<ClanShopItem> items)
        {
            renderedItems.Clear();
            if (items != null) renderedItems.AddRange(items.Where(item => item?.Item != null && item.Item.Count > 0));
            if (selectedItem != null) selectedItem = renderedItems.FirstOrDefault(item => item.Id == selectedItem.Id);
            if (emptyState != null) emptyState.SetActive(renderedItems.Count == 0);
            UpdateContributionPoints();

            if (itemList != null)
            {
                renderedItemRefs.Clear();
                renderedItemRefs.AddRange(renderedItems.Select(item => Clone(item.Item)));
                itemList.InitDataItem(renderedItemRefs, BindRenderedItem, false, true);
                itemList.itemDataShows = renderedItemRefs;
                itemList.InitBag();
            }
            UpdateSelectedView();
        }

        private void BindRenderedItem(ItemInventory itemView)
        {
            var index = renderedItemRefs.FindIndex(item => ReferenceEquals(item, itemView.itemRef));
            if (index < 0 || index >= renderedItems.Count) return;
            var shopItem = renderedItems[index];
            itemView.InitAction(null, null, () => Select(shopItem));
        }

        private void OnClanChanged(Network.Models.Clans.Clan clan)
        {
            if (clan == null) Hide();
            else Render(clan.ShopItems);
        }

        private void OnShopChanged(IReadOnlyList<ClanShopItem> items) => Render(items);

        private void OnMemberPointsChanged(ClanMemberPointsChanged changed)
        {
            if (changed.CharId == (Client.Instance?.CharacterData?.Id ?? 0)) UpdateContributionPoints();
        }

        private void OnContributionChanged(ClanContribution contribution)
        {
            if (contribution.CharId == (Client.Instance?.CharacterData?.Id ?? 0)) UpdateContributionPoints();
        }

        private void OpenBag()
        {
            if (clanBag != null) clanBag.Show();
            else Notify("Chưa cấu hình ClanBag");
        }

        private void OnQuantityChanged(string _)
        {
            UpdatePrice();
            UpdateButtons();
        }

        private void UpdateSelectedView()
        {
            var hasSelection = selectedItem?.Item != null;
            if (selectedItemText != null)
                selectedItemText.text = hasSelection ? $"{selectedItem.Item.Type} #{selectedItem.Item.Id}" : "Chưa chọn vật phẩm";
            if (priceText != null) priceText.text = hasSelection ? selectedItem.Price.ToString("N0") : "0";
            UpdatePrice();
            UpdateButtons();
        }

        private void UpdatePrice()
        {
            if (totalPriceText == null) return;
            if (selectedItem == null || selectedItem.Price < 0)
            {
                totalPriceText.text = "0";
                return;
            }
            var quantity = Math.Max(1, ReadQuantity());
            totalPriceText.text = selectedItem.Price > 0 && quantity > long.MaxValue / selectedItem.Price
                ? "Quá lớn"
                : (selectedItem.Price * quantity).ToString("N0");
        }

        private void UpdateContributionPoints()
        {
            if (contributionPointsText != null) contributionPointsText.text = ContributionPoints.ToString("N0");
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            var hasSelection = selectedItem != null;
            var handlerReady = Handler != null;
            if (refreshButton != null) refreshButton.interactable = !isLoading && !isSubmitting;
            if (buyButton != null)
                buyButton.interactable = handlerReady && !isLoading && !isSubmitting && hasSelection &&
                                         Handler.MaxShopQuantityPerRequest > 0 && !Handler.IsShopItemOwner(selectedItem);
            if (removeButton != null)
                removeButton.interactable = handlerReady && !isLoading && !isSubmitting && hasSelection &&
                                            Handler.IsShopItemOwner(selectedItem) && HasPermission(ClanPermission.RemoveShopItem);
        }

        private bool HasPermission(ClanPermission permission)
        {
            if (Handler?.CurrentClan?.Members == null) return false;
            var characterId = Client.Instance?.CharacterData?.Id ?? 0;
            var member = Handler.CurrentClan.Members.FirstOrDefault(value => value.CharId == characterId);
            return member != null && (Handler.GetPermissions(member.Role) & permission) == permission;
        }

        private int ReadQuantity() => quantityInput != null && int.TryParse(quantityInput.text, out var value) ? value : 1;
        private void SetQuantity(int quantity) { if (quantityInput != null) quantityInput.text = Math.Max(1, quantity).ToString(); }
        private void SetLoading(bool value) { if (loadingState != null) loadingState.SetActive(value); UpdateButtons(); }
        private static ItemRef Clone(ItemRef item) => new(item.Id, item.Type, item.Count, item.TempId);
        private static void Notify(string message) { if (!string.IsNullOrWhiteSpace(message)) NotifyGroup.Instance?.Show(message); }

        protected override void OnDestroy()
        {
            if (quantityInput != null) quantityInput.onValueChanged.RemoveListener(OnQuantityChanged);
            SelectionChanged = null;
            base.OnDestroy();
        }
    }
}
