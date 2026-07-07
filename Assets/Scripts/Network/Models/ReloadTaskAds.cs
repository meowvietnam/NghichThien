using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ReloadTaskAds : ShowAds
    {
        public ReloadTaskAds(string location, string buttonName) : base(location, buttonName)
        {
        }

        public override ShowAdsType GetAdsType()
        {
            return ShowAdsType.ReloadTask;
        }
    }
}