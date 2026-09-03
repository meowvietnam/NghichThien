using LuyenDan;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class ItemBag : LuyenDan.BaseItemUI
    {
        [SerializeField, TabGroup("Has Item")] private Button selectButton;
        [SerializeField, TabGroup("Has Item")] private Button infoButton;
        public ItemRef ItemRef { get; private set; }
        public Material Material { get; private set; }
        private bool IsHasItem { get; set; }

        protected virtual void Init(IItemData data)
        {
            IsHasItem = data != null;
            Material = data as Material;
            SetData(data);
            if (IsHasItem)
            {
                SetEventButton(selectButton, OnSelected);
                SetEventButton(infoButton, ShowInfo);
            }
            else
            {
                infoButton.gameObject.SetActive(false);
            }
        }

        private void ShowInfo()
        {
            ToolTipItems.Instance.ShowInfoItem(Material, ItemRef);
        }

        private void OnSelected()
        {
            LuyenKhiPanel.Instance.Bag.OnSelect(this);
        }

        public virtual void Init(ItemRef itemRef)
        {
            ItemRef = itemRef;
            SetCount(itemRef.Count);
            var data = LuyenKhiResources.Instance.FindItem(itemRef);
            if (data != null)
            {
                itemRef.OnCountChanged += OnCountChanged;
            }

            Init(data);
        }

        private void OnCountChanged(int count)
        {
            SetCount(count);
            SetActive(count > 0);
        }

        private void OnDestroy()
        {
            if (IsHasItem && ItemRef != null)
            {
                ItemRef.OnCountChanged -= OnCountChanged;
                ClearListener();
            }
        }

        public void ClearListener()
        {
            selectButton.onClick.RemoveAllListeners();
        }

        public void Destroy()
        {
            Destroy(selfObject);
        }
    }
}