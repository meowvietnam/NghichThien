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
        [SerializeField] private InputField priceInput;
        [SerializeField] private Button addButton;

        private readonly List<ItemRef> items = new();
        private ItemRef selectedItem;
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
            if (priceInput != null)
            {
                priceInput.onValueChanged.RemoveListener(OnInputChanged);
                priceInput.onValueChanged.AddListener(OnInputChanged);
            }
            UpdateView();
        }

        protected override void BindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged += OnClanChanged;
            handler.ShopChanged += OnShopChanged;
            var character = Client.Instance?.CharacterData;
            if (character != null) character.OnInventoryUpdated += OnInventoryUpdated;
        }

        protected override void UnbindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged -= OnClanChanged;
            handler.ShopChanged -= OnShopChanged;
            var character = Client.Instance?.CharacterData;
            if (character != null) character.OnInventoryUpdated -= OnInventoryUpdated;
        }

        protected override void OnShown()
        {
            RefreshInventory();
        }

        public void Select(ItemRef item)
        {
            selectedItem = item;
            if (quantityInput != null) quantityInput.text = "1";
            UpdateView();
            SelectionChanged?.Invoke(selectedItem);
        }

        public void AddSelectedItem()
        {
            if (!isSubmitting) AddSelectedItemAsync();
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

            if (selectedItem != null) selectedItem = items.FirstOrDefault(value => value.Equals(selectedItem));
            Render();
        }

        private async void AddSelectedItemAsync()
        {
            if (!Validate(out var quantity, out var price, out var message))
            {
                Notify(message);
                return;
            }

            var requestItem = new ItemRef(selectedItem.Id, selectedItem.Type, quantity, selectedItem.TempId);
            isSubmitting = true;
            UpdateButton();
            try
            {
                var result = await Handler.AddShopItem(requestItem, price);
                Notify(result.Message);
                if (result.Success)
                {
                    selectedItem = null;
                    RefreshInventory();
                    SelectionChanged?.Invoke(null);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                Notify("Không thể thêm vật phẩm vào kho tông môn");
            }
            finally
            {
                isSubmitting = false;
                UpdateView();
            }
        }

        private bool Validate(out int quantity, out long price, out string message)
        {
            quantity = 0;
            price = 0;
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
            else if (priceInput == null || !long.TryParse(priceInput.text, out price) || price < 1)
                message = "Giá phải lớn hơn 0";
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
                    var item = items.FirstOrDefault(value => value.Equals(itemView.itemRef));
                    itemView.InitAction(null, null, () => Select(item));
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
                    : $"{selectedItem.Type} #{selectedItem.Id} (có {selectedItem.Count})";
            UpdateDailyLimit();
            UpdateButton();
        }

        private void UpdateDailyLimit()
        {
            if (dailyLimitText == null) return;
            var maximum = Handler?.MaxShopItemsAddedPerDay ?? 0;
            var used = Handler?.ShopItemsAddedToday ?? 0;
            dailyLimitText.text = maximum > 0 ? $"Hôm nay: {used}/{maximum}" : "Kho đang tạm khóa";
        }

        private void UpdateButton()
        {
            if (addButton == null) return;
            var maximum = Handler?.MaxShopItemsAddedPerDay ?? 0;
            var used = Handler?.ShopItemsAddedToday ?? 0;
            addButton.interactable = !isSubmitting && selectedItem != null && maximum > 0 && used < maximum &&
                                     HasPermission(ClanPermission.AddShopItem);
        }

        private bool HasPermission(ClanPermission permission)
        {
            if (Handler?.CurrentClan?.Members == null) return false;
            var characterId = Client.Instance?.CharacterData?.Id ?? 0;
            var member = Handler.CurrentClan.Members.FirstOrDefault(value => value.CharId == characterId);
            return member != null && (Handler.GetPermissions(member.Role) & permission) == permission;
        }

        private static ItemRef Clone(ItemRef item) => new(item.Id, item.Type, item.Count, item.TempId);
        private static void Notify(string message) { if (!string.IsNullOrWhiteSpace(message)) NotifyGroup.Instance?.Show(message); }

        protected override void OnDestroy()
        {
            if (quantityInput != null) quantityInput.onValueChanged.RemoveListener(OnInputChanged);
            if (priceInput != null) priceInput.onValueChanged.RemoveListener(OnInputChanged);
            SelectionChanged = null;
            base.OnDestroy();
        }
    }
}
