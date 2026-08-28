using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;

namespace ConvertElementalSkill.Prefabs
{
    public class LinhDanItem : BaseItemUI
    {
        private IItemData itemData;
        private ItemRef itemRef;
        public override void SetData(IItemData itemData, ItemRef itemRef)
        {
            base.SetData(itemData, itemRef);
            this.itemData = itemData;
            this.itemRef = itemRef;
        }

        protected override void OnShowInfo()
        {
            ToolTipItems.Instance.ShowInfoItem(itemData,itemRef);
        }
    }
}
