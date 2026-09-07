using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ArenaAds : ShowAds
    {
        public ArenaAds(string location, string buttonName) : base(location, buttonName)
        {
        }

        public override ShowAdsType GetAdsType()
        {
            return ShowAdsType.Arena;
        }
    }
}