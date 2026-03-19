using UnityEngine;

namespace LuyenDan.Inventory
{
    public class FurnaceItemListUI : ItemListUI<FurnaceInventoryItemUI>
    {
        public override void Filter(Tier tier)
        {
            Debug.Log(tier);
            var selectedFurnace = LuyenDanPanel.Instance.Alchermy.SelectedFurnace;
            foreach (var item in Items)
            {
                var active = item.IsHasItem && item.Tier == tier && item != selectedFurnace;
                item.SetActive(active);
            }
        }
        public override void NoFilter()
        {
            var selectedFurnace = LuyenDanPanel.Instance.Alchermy.SelectedFurnace;
            foreach (var item in Items)
            {
                item.SetActive(selectedFurnace != item);
            }
        }
    }
}
