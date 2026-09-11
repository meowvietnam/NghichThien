using LuyenDan;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertDanLu
{
    /// <summary>
    /// One of the 4 input slots that the player places a đan phương (recipe) into before converting.
    /// </summary>
    public class Slot : BaseItemUI, ISelectedItemCallback
    {
        [SerializeField] private Button clearButton;

        protected override void Start()
        {
            base.Start();
            SetEventButton(clearButton, Clear);
        }

        public void Clear()
        {
            ClearItem();
            ConvertDanLuPanel.Instance.Page.PutItems.OnDeselect(this);
            ClearNew();
        }

        private void ClearItem()
        {
            if (DanData == null) return;

            var bag     = ConvertDanLuPanel.Instance.Bag;
            var myItem  = DanData.ItemRef;
            var itemBag = bag.FindItem((ushort)myItem.Id);
            if (itemBag != null)
            {
                var itemRef = itemBag.DanData.ItemRef;
                itemRef.SetCount(itemRef.Count + myItem.Count);
                bag.ItemList.ApplyFilterOn(itemBag);
            }
        }

        public void ClearNew()
        {
            SetData(null);
            clearButton.gameObject.SetActive(false);
        }

        protected override void OnSelected()
        {
            ConvertDanLuPanel.Instance.Bag.OpenWithCallback(this);
        }

        protected override void OnShowInfo()
        {
            if (DanData == null) return;
            var itemRef    = DanData.ItemRef;
            var recipeData = LuyenDanResources.Instance.FindRecipe(itemRef.Id);
            if (recipeData == null) return;
            ToolTipItems.Instance.ShowInfoItem(recipeData, itemRef);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            clearButton.onClick.RemoveAllListeners();
        }

        // Called back when the player picks a recipe from the Bag
        public void OnSelected(ItemBag item)
        {
            ClearItem();

            var instance = ConvertDanLuPanel.Instance;
            var itemRef  = item.DanData.ItemRef;
            var cloned   = itemRef.DeepClone();
            cloned.SetCount(1);
            itemRef.SetCount(itemRef.Count - 1);

            var danData = new DanItemData(cloned, item.DanData.ItemData);
            SetData(danData);
            instance.Bag.ItemList.ApplyFilterOn(item);
            instance.Page.PutItems.OnSelect(this);
            clearButton.gameObject.SetActive(true);
        }
    }
}
