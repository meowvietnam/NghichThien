using System.Collections.Generic;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class SpecialLevelUpResult : Result
    {
        public StatsSnapshot StatsSnapshot { get; set; }
        public List<ItemRef> BuffItems { get; set; }
    }
}