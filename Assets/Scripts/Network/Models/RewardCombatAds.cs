using MemoryPack;
using Network.Core;

namespace Network.Models
{
    [MemoryPackable]
    public partial class RewardCombatAds : ShowAds
    {
        public RewardCombatAds(string location, string buttonName) : base(location, buttonName)
        {
        }

        public override ShowAdsType GetAdsType()
        {
            return ShowAdsType.RewardCombat;
        }
    }
}