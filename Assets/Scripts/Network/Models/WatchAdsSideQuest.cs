using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class WatchAdsSideQuest : ShowAds
    {
        public WatchAdsSideQuest(string location, string buttonName) : base(location, buttonName)
        {
        }

        public override ShowAdsType GetAdsType()
        {
            return ShowAdsType.SideQuest;
        }
    }
}