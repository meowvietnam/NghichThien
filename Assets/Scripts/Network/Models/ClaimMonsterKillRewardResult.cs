using System.Collections.Generic;
using MemoryPack;
using Rank;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ClaimMonsterKillRewardResult : Result
    {
        public long Id { get; set; }
        public List<IEventReward> Rewards { get; set; }
    }
}