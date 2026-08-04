using AllMaps.EventReward;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class WatchAdsCombatResult : Result
    {
        public RewardSnapshot Reward { get; set; }
    }
}