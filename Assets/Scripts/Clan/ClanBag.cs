using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreateCharacter;
using LuyenDan;
using Network;
using Network.Core;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;

namespace Clan
{
    /// <summary>Chọn vật phẩm trong túi người chơi để đưa vào kho tông môn.</summary>
    public class ClanBag : ClanPopup
    {
        [Header("Túi đồ")]
        [SerializeField] private Bag1ItemPrefabInventory itemList;
        [SerializeField] private GameObject emptyState;

        [Header("Thông tin đăng bán")]
        [SerializeField] private Text selectedItemText;
        [SerializeField] private Text dailyLimitText;
        [SerializeField] private InputField quantityInput;
        [SerializeField] private Button addButton;

        private readonly List<ItemRef> items = new();
        private ItemRef selectedItem;
        private CharacterData observedCharacter;
        private bool isSubmitting;

        public IReadOnlyList<ItemRef> Items => items;
        public ItemRef SelectedItem => selectedItem;
        public event Action<ItemRef> SelectionChanged;

        protected override void Start()
        {
            base.Start();
            SetEventButton(addButton, AddSelectedItem);
            if (quantityInput != null)
            {
                quantityInput.onValueChanged.RemoveListener(OnInputChanged);
                quantityInput.onValueChanged.AddListener(OnInputChanged);
            }
            UpdateView();
        }

        protected override void BindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged += OnClanChanged;
            handler.ShopChanged += OnShopChanged;
            observedCharacter = Client.Instance?.CharacterData;
            if (observedCharacter != null) observedCharacter.OnInventoryUpdated += OnInventoryUpdated;
        }

        protected override void UnbindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged -= OnClanChanged;
            handler.ShopChanged -= OnShopChanged;
            if (observedCharacter != null) observedCharacter.OnInventoryUpdated -= OnInventoryUpdated;
            observedCharacter = null;
        }

        protected override void OnShown()
        {
            RefreshInventory();
        }

        public void Select(ItemRef item)
        {
            var selection = item == null
                ? null
                : items.FirstOrDefault(value => SameItem(value, item));
            SetSelection(selection, true);
        }

        public void AddSelectedItem()
        {
            if (!isSubmitting) _ = AddSelectedItemAsync();
        }

        public void RefreshInventory()
        {
            items.Clear();
            var inventory = Client.Instance?.CharacterData?.InventoryManager;
            if (inventory != null)
            {
                AddRange(inventory.Herbs);
                AddRange(inventory.Recipes);
                AddRange(inventory.ThanThongs);
                AddRange(inventory.CongPhaps);
                AddRange(inventory.LinhDans);
                AddRange(inventory.Materials);
            }

            if (selectedItem != null)
            {
                var refreshedSelection = items.FirstOrDefault(value => SameItem(value, selectedItem));
                if (!ReferenceEquals(refreshedSelection, selectedItem))
                {
                    selectedItem = refreshedSelection;
                    SelectionChanged?.Invoke(selectedItem);
                }
            }
            Render();
        }

        private async Task AddSelectedItemAsync()
        {
            if (!Validate(out var quantity, out var message))
            {
                Notify(message);
                return;
            }

            var requestItem = new ItemRef(selectedItem.Id, selectedItem.Type, quantity, selectedItem.TempId);
            SetSubmitting(true);
            try
            {
                var result = await Handler.AddShopItem(requestItem);
                Notify(string.IsNullOrWhiteSpace(result.Message)
                    ? result.Success ? "Đã thêm vật phẩm vào kho tông môn" : "Không thể thêm vật phẩm vào kho tông môn"
                    : result.Message);
                if (result.Success)
                {
                    SetSelection(null, false);
                    RefreshInventory();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                Notify("Không thể thêm vật phẩm vào kho tông môn");
            }
            finally
            {
                SetSubmitting(false);
            }
        }

        private bool Validate(out int quantity, out string message)
        {
            quantity = 0;
            message = string.Empty;
            if (Handler == null || Handler.CurrentClan == null) message = "Bạn chưa thuộc tông môn";
            else if (!HasPermission(ClanPermission.AddShopItem)) message = "Bạn không có quyền thêm vật phẩm vào kho";
            else if (Handler.MaxShopItemsAddedPerDay <= 0) message = "Kho tông môn đang tạm khóa thêm vật phẩm";
            else if (selectedItem == null) message = "Hãy chọn vật phẩm muốn thêm";
            else if (quantityInput == null || !int.TryParse(quantityInput.text, out quantity) || quantity < 1)
                message = "Số lượng phải lớn hơn 0";
            else if (quantity > selectedItem.Count) message = "Túi đồ không đủ số lượng";
            else if ((long)Handler.ShopItemsAddedToday + quantity > Handler.MaxShopItemsAddedPerDay)
                message = "Số lượng đã vượt giới hạn thêm vật phẩm hôm nay";
            return string.IsNullOrEmpty(message);
        }

        private void Render()
        {
            if (emptyState != null) emptyState.SetActive(items.Count == 0);
            if (itemList != null)
            {
                var renderItems = items.Select(Clone).ToList();
                itemList.InitDataItem(renderItems, itemView =>
                {
                    var item = items.FirstOrDefault(value => SameItem(value, itemView.itemRef));
                    if (item != null) itemView.InitAction(null, null, () => Select(item));
                }, false, true);
                itemList.InitBag();
            }
            UpdateView();
        }

        private void AddRange(IEnumerable<ItemRef> source)
        {
            if (source != null) items.AddRange(source.Where(IsAvailable));
        }

        private static bool IsAvailable(ItemRef item) => item != null && item.Count > 0;

        private void OnClanChanged(Network.Models.Clans.Clan clan)
        {
            if (clan == null) Hide();
            else UpdateView();
        }

        private void OnShopChanged(IReadOnlyList<ClanShopItem> _) => UpdateDailyLimit();
        private void OnInventoryUpdated(InventoryManager _) => RefreshInventory();
        private void OnInputChanged(string _) => UpdateButton();

        private void UpdateView()
        {
            if (selectedItemText != null)
                selectedItemText.text = selectedItem == null
                    ? "Chưa chọn vật phẩm"
                    : $"{ClanShop.GetItemName(selectedItem)} (có {selectedItem.Count:n0})";
            UpdateDailyLimit();
            UpdateButton();
        }

        private void UpdateDailyLimit()
        {
            if (dailyLimitText == null) return;
            var maximum = Handler?.MaxShopItemsAddedPerDay ?? 0;
            var used = Handler?.ShopItemsAddedToday ?? 0;
            var remaining = Math.Max(0, maximum - used);
            dailyLimitText.text = maximum > 0
                ? $"Hôm nay: {used}/{maximum} · Còn {remaining}"
                : "Kho đang tạm khóa";
        }

        private void UpdateButton()
        {
            if (addButton == null) return;
            addButton.interactable = !isSubmitting && CanSubmitCurrentInput();
        }

        private bool CanSubmitCurrentInput()
        {
            if (Handler?.CurrentClan == null || selectedItem == null ||
                !HasPermission(ClanPermission.AddShopItem)) return false;

            var maximum = Handler.MaxShopItemsAddedPerDay;
            var used = Handler.ShopItemsAddedToday;
            if (maximum <= 0 || used >= maximum) return false;
            if (quantityInput == null || !int.TryParse(quantityInput.text, out var quantity) ||
                quantity < 1 || quantity > selectedItem.Count || (long)used + quantity > maximum) return false;
            return true;
        }

        private void SetSelection(ItemRef item, bool resetQuantity)
        {
            if (ReferenceEquals(selectedItem, item) && !resetQuantity) return;
            selectedItem = item;
            if (resetQuantity && quantityInput != null) quantityInput.text = "1";
            UpdateView();
            SelectionChanged?.Invoke(selectedItem);
        }

        private void SetSubmitting(bool value)
        {
            isSubmitting = value;
            if (quantityInput != null) quantityInput.interactable = !value;
            UpdateView();
        }

        private bool HasPermission(ClanPermission permission)
        {
            if (Handler?.CurrentClan?.Members == null) return false;
            var characterId = Client.Instance?.CharacterData?.Id ?? 0;
            var member = Handler.CurrentClan.Members.FirstOrDefault(value => value.CharId == characterId);
            return member != null && (Handler.GetPermissions(member.Role) & permission) == permission;
        }

        private static bool SameItem(ItemRef left, ItemRef right) =>
            left != null && right != null && left.Id == right.Id && left.Type == right.Type && left.TempId == right.TempId;

        private static ItemRef Clone(ItemRef item) => new(item.Id, item.Type, item.Count, item.TempId);
        private static void Notify(string message) { if (!string.IsNullOrWhiteSpace(message)) NotifyGroup.Instance?.Show(message); }

        protected override void OnDestroy()
        {
            if (quantityInput != null) quantityInput.onValueChanged.RemoveListener(OnInputChanged);
            SelectionChanged = null;
            base.OnDestroy();
        }
    }
}
