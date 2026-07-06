using System;
using System.Collections.Generic;
using MemoryPack;

namespace CreateCharacter
{
    [MemoryPackable]
    [Serializable]
    public partial class ArenaManager
    {
        public int CurrentArenaCount { get; set; }
        public int Points { get; set; }
        public List<RankReward> Rewards { get; set; } = new();
    }
}