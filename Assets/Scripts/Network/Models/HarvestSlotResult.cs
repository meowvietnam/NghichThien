using MemoryPack;
using DP;
using System.Collections.Generic;

namespace Network.Models
{
    [MemoryPackable]
    public partial class HarvestSlotNotify
    {
        public ushort SlotIndex { get; set; }
    }

    [MemoryPackable]
    public partial class HarvestSlotResult : Result
    {
        public ushort SlotIndex { get; set; }
        public List<IDongPhuReward> Rewards { get; set; }
    }
}