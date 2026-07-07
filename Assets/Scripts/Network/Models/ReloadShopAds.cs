using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ReloadShopAds : ShowAds
    {
        public ReloadShopAds(string location, string buttonName) : base(location, buttonName)
        {
        }

        public override ShowAdsType GetAdsType()
        {
            return ShowAdsType.ReloadShop;
        }
    }
}