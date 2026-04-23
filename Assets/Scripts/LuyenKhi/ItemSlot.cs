using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class ItemSlot : LuyenDan.BaseItemUI, ISelectItemBagCallback
    {
        public ItemBag CurrentSelect { get;private set; }
        [SerializeField]
        private Button button;
        [SerializeField]
        private PutMaterials putMaterials;
        [SerializeField,TabGroup("Has Item")]
        private Button clearButton;

        private void OnValidate()
        {
            clearButton = transform.Find("Clear").GetComponent<Button>();
            clearButton.gameObject.SetActive(false);
        }
        private void Start()
        {
            SetEventButton(button,OnClick);
            SetEventButton(clearButton,OnClear);
        }

        private void OnClear()
        {
            Select(null);
        }

        private void OnClick()
        {
            LuyenKhiPanel.Instance.Bag.OpenWithCallback(this);
        }
        public void Select(ItemBag item)
        {
            bool isOtherItem = CurrentSelect != item;
            if (!isOtherItem)
            {
                return;
            }
            if (CurrentSelect != null)
            {
                var itemRef = CurrentSelect.ItemRef;
                itemRef.SetCount(itemRef.Count + 1);
            }
            CurrentSelect = item;
            var isItemNull = item == null;
            if (!isItemNull)
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
            putMaterials.Recalculate();
            clearButton.gameObject.SetActive(!isItemNull);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}