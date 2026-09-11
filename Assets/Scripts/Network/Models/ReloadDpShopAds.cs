using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ReloadDpShopAds : ShowAds
    {
        public ReloadDpShopAds(string location, string buttonName) : base(location, buttonName)
        {
        }

        public override ShowAdsType GetAdsType()
        {
            return ShowAdsType.ReloadDpShop;
        }
    }
}