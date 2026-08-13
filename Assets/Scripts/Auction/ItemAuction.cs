using AllMaps.ItemProvider;
using Base;
using LongDHModules;
using LuyenDan;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Auction
{
    public class ItemAuction : BaseBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, TabGroup("Has Item")] private Button infoButton;
        [SerializeField] private GameObject selfObject;

        [TabGroup("Null Item")] [SerializeField]
        private GameObject emptySlot;

        [TabGroup("Has Item")] [SerializeField]
        private GameObject filledSlot;

        [TabGroup("Has Item")] [SerializeField]
        private Image itemIcon;

        [TabGroup("Has Item")] [SerializeField]
        private Image qualityBackground;

        [TabGroup("Has Item")] [SerializeField]
        private Image qualityIconUp;

        [TabGroup("Has Item")] [SerializeField]
        private Text countText;

        [TabGroup("Has Item")] [SerializeField]
        private Text itemNameText;

        [TabGroup("Has Item")] [SerializeField]
        private GameObject hoverHighlight;

        private Item item;
        [Button]
        public virtual void GetReference()
        {
            selfObject = gameObject;
            emptySlot = transform.Find("Null").gameObject;
            filledSlot = transform.Find("HasItem").gameObject;
            itemIcon = transform.Find("HasItem/IconMask/Icon").GetComponent<Image>();
            qualityBackground = transform.Find("HasItem/Quality").GetComponent<Image>();
            qualityIconUp = transform.Find("HasItem/Quality/QualityUp").GetComponent<Image>();
            countText = transform.Find("HasItem/CountText").GetComponent<Text>();
            itemNameText = transform.Find("HasItem/NameMask/NameText").GetComponent<Text>();
            hoverHighlight = transform.Find("Selected").gameObject;
        }

        public void SetActive(bool active)
        {
            if (selfObject != null)
                selfObject.SetActive(active);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverHighlight != null)
                hoverHighlight.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (hoverHighlight != null)
                hoverHighlight.SetActive(false);
        }

        public void SetCount(int count)
        {
            if (countText != null)
            {
                countText.text = count > 1 ? Utils.FormatCount(count) : "";
            }
        }

        public void SetItem(Item item)
        {
            this.item = item;
            bool hasItem = item != null;
            if (emptySlot != null) emptySlot.SetActive(!hasItem);
            if (filledSlot != null) filledSlot.SetActive(hasItem);
            if (!hasItem)
            {
                infoButton.gameObject.SetActive(false);
                return;
            }
            var data = item.Data;
            if (itemIcon != null) itemIcon.sprite = data.Icon;
            if (itemNameText != null)
            {
                itemNameText.text = data.Name;
                itemNameText.color = data.Color;
            }
            if (qualityIconUp != null) qualityIconUp.sprite = data.QualityIconUp;
            if (qualityBackground != null) qualityBackground.sprite = data.QualityBackground;
            SetCount(item.Ref.Count);
            infoButton.gameObject.SetActive(true);
            SetEventButton(infoButton, OnShowInfo);
            
        }
        protected virtual void OnSelected()
        {
        }

        protected virtual void OnShowInfo()
        {
            var toolTip = ToolTipItems.Instance;
            item.Data.ShowInfo(toolTip);
        }
        
        public void DestroySelf()
        {
            Destroy(selfObject);
        }
    }
}