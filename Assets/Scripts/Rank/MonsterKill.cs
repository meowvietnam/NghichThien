using System.Collections.Generic;
using MemoryPack;

namespace Rank
{
    [MemoryPackable]
    public partial class MonsterKill
    {
        public int Points { get; private set; }
        public List<MonsterRankReward> Rewards { get; private set; } = new();
    }
}