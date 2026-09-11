using System;
using Base;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class DongPhuSeedConfig
    {
        public Tier Tier { get; set; }
        public TimeSpan GrowthDuration { get; set; }
    }
}