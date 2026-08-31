using System.Collections.Generic;
using MemoryPack;
using Rank;

namespace Network.Models
{
    [MemoryPackable]
    public partial class MilestoneClaimResult : Result, IEventResultData
    {
        public List<IEventReward> Rewards { get; set; }
        public bool IsSuccess()
        {
            return Success;
        }
    }
}