using System;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class DongPhuConfig
    {
        public DongPhuLevelConfig[] Levels { get; private set; }
        public DongPhuSeedConfig[] Seeds { get; private set; }

        public bool IsLevelGreaterOrEqual(DongPhuLevel levelA, DongPhuLevel levelB)
        {
            var indexA = Array.FindIndex(Levels, x => x.Level == levelA);
            var indexB = Array.FindIndex(Levels, x => x.Level == levelB);
            return indexA >= indexB;
        }
    }
}