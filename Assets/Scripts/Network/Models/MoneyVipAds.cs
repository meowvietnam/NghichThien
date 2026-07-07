using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class MoneyVipAds : ShowAds
    {
        public MoneyVipAds(string location, string buttonName) : base(location, buttonName)
        {
        }

        public override ShowAdsType GetAdsType()
        {
            return ShowAdsType.MoneyVip;
        }
    }
}