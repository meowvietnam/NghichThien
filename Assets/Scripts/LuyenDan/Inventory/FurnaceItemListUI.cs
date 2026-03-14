namespace LuyenDan.Inventory
{
    public class FurnaceItemListUI : ItemListUI<FurnaceInventoryItemUI>
    {
        public override void Filter(Tier tier)
        {
            foreach (var item in Items)
            {
                var active = item.IsHasItem && item.Tier == tier;
                item.SetActive(active);
            }
        }
        public override void NoFilter()
        {
            foreach (var item in Items)
            {
                item.SetActive(true);
            }
        }
    }
}
