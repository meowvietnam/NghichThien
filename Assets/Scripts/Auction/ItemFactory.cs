using LuyenDan;

namespace Auction
{
    public static class ItemFactory
    {
        public static Item CreateItem(ItemRef itemRef)
        {
            var id = itemRef.Id;
            var type = itemRef.Type;
            IItemAuctionData data = type switch
            {
                ItemType.Herb => new HerbData(id),
                ItemType.Furnace => new FurnaceData(id),
                ItemType.LinhDan => new LinhDanData(id),
                ItemType.Material => new MaterialData(id),
                ItemType.CongPhap => new CongPhapData(id),
                ItemType.ThanThong => new ThanThongData(id),
                ItemType.Recipe => new RecipeData(id),
                _ => throw new($"Không hỗ trợ {type}")
            };
            return new Item(itemRef, data);
        }
    }
}