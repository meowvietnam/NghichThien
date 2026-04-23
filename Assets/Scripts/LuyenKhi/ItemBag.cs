using LuyenDan;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class ItemBag : LuyenDan.BaseItemUI
    {
        [SerializeField,TabGroup("Has Item")] private Button selectButton;
        private ItemRef itemRef;
        public Material Material { get; private set; }
        private bool IsHasItem { get; set; }
        protected virtual void Init(IItemData data)
        {
            IsHasItem = data != null;
            Material = data as Material;
            SetData(data);
            if (IsHasItem)
            {
                selectButton.onClick.AddListener(OnSelected);
            }
        }
        private void OnSelected()
        {
            Debug.Log("OnSelected");
        }
        public virtual void Init(ItemRef itemRef)
        {
            this.itemRef = itemRef;
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
            if (IsHasItem && itemRef != null)
            {
                itemRef.OnCountChanged -= OnCountChanged;
                selectButton.onClick.RemoveListener(OnSelected);
            }
        }
    }
}