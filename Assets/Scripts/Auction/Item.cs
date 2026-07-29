using LuyenDan;

namespace Auction
{
    public class Item
    {
        public ItemRef Ref { get; private set; }
        public IItemAuctionData Data { get; private set; }

        public Item(ItemRef refItem, IItemAuctionData data)
        {
            Ref = refItem;
            Data = data;
        }
    }
}