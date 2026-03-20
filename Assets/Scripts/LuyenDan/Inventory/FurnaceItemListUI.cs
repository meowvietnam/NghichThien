using UnityEngine;

namespace LuyenDan.Inventory
{
    public class FurnaceItemListUI : ItemListUI<FurnaceInventoryItemUI>
    {
        public override void Filter(Tier tier)
        {
            var selectedFurnace = LuyenDanPanel.Instance.Alchermy.SelectedFurnace;
            foreach (var item in Items)
            {
                var active = item.IsHasItem && item.Tier == tier && item != selectedFurnace && item.ItemRef.Durability > 0;
                item.SetActive(active);
            }
        }
        public override void NoFilter()
        {
            var selectedFurnace = LuyenDanPanel.Instance.Alchermy.SelectedFurnace;
            foreach (var item in Items)
            {
                item.SetActive(selectedFurnace != item && item.ItemRef.Durability > 0);
            }
        }
    }
}
