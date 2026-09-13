using MemoryPack;
using Rank;

namespace Events
{
    [MemoryPackable]
    public partial class ItemEventShop
    {
        public ushort Id { get; set; }
        public bool HasBuy { get; set; }
        public IEventReward Reward { get; set; }
        public int Amount { get; set; }
        public int MoneyVip { get; set; }
        public int BonusMoneyVip { get; set; }
    }
}
