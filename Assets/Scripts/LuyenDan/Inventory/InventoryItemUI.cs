using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public class InventoryItemUI : BaseItemUI
    {
        [TabGroup("Has Item")] [SerializeField]
        private Button infoButton;

        [TabGroup("Has Item")] [SerializeField]
        private Button selectButton;

        private ItemRef itemRef;
        [ShowInInspector, ReadOnly] public ItemRef ItemRef => itemRef;

        public bool IsHasItem { get; private set; }
        // private void OnValidate()
        // {
        //     selfObject   = gameObject;
        //
        //     var tf = selfObject.transform;
        //     emptySlot  = tf.Find("Null").gameObject;
        //     filledSlot = tf.Find("HasItem").gameObject;
        //
        //     itemIcon          = filledSlot.transform.Find("IconMask/Icon").GetComponent<Image>();
        //     qualityBackground = filledSlot.transform.Find("Quality").GetComponent<Image>();
        //     qualityIconUp     = qualityBackground.transform.Find("QualityUp").GetComponent<Image>();
        //     hoverHighlight    = tf.Find("HasItem/Selected").gameObject;
        //     itemNameText      = filledSlot.transform.Find("NameMask/NameText").GetComponent<Text>();
        // }

        protected virtual void Init(IItemData data)
        {
            IsHasItem = data != null;
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

        protected virtual void ShowInfo()
        {
        }

        public virtual void Init(ItemRef itemRef)
        {
            this.itemRef = itemRef;
            SetCount(itemRef.Count);
            var data = LuyenDanPanel.Instance.Resources.FindItem(itemRef);
            if (data != null)
            {
                itemRef.OnCountChanged += OnCountChanged;
            }

            Init(data);
        }

        protected virtual void OnSelected()
        {
            Debug.Log("OnSelected");
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

        public void Destroy()
        {
            Destroy(selfObject);
        }

        public virtual IItemData GetData()
        {
            return null;
        }
    }
}