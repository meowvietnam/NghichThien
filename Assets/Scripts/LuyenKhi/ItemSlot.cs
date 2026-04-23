using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class ItemSlot : LuyenDan.BaseItemUI, ISelectItemBagCallback
    {
        private ItemBag currentSelect;
        // private void OnValidate()
        // {
        //     selfObject = gameObject;
        //     emptySlot = transform.Find("Null").gameObject;
        //     filledSlot = transform.Find("HasItem").gameObject;
        //     itemIcon = filledSlot.transform.Find("IconMask/Icon").GetComponent<Image>();
        //     qualityBackground = filledSlot.transform.Find("Quality").GetComponent<Image>();
        //     qualityIconUp = qualityBackground.transform.Find("QualityUp").GetComponent<Image>();
        //     countText = filledSlot.transform.Find("CountText").GetComponent<Text>();
        //     itemNameText = filledSlot.transform.Find("NameMask/NameText").GetComponent<Text>();
        //     hoverHighlight = transform.Find("Selected").gameObject;
        // }
        [SerializeField]
        private Button button;

        private void OnValidate()
        {
            button = GetComponent<Button>();
        }
        private void Start()
        {
            SetEventButton(button,OnClick);
        }
        private void OnClick()
        {
            LuyenKhiPanel.Instance.Bag.OpenWithCallback(this);
        }
        public void Select(ItemBag item)
        {
            bool isOtherItem = currentSelect != item;
            if (!isOtherItem)
            {
                return;
            }
            if (currentSelect != null)
            {
                var itemRef = currentSelect.ItemRef;
                itemRef.SetCount(itemRef.Count + 1);
            }
            currentSelect = item;
            if (item != null)
            {
                SetData(item.Material);
                SetCount(1);
                var itemRef = item.ItemRef;
                itemRef.SetCount(itemRef.Count - 1);
            }
            else
            {
                SetData(null);
            }
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}