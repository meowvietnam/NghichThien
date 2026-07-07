using System;
using System.Collections.Generic;
using MemoryPack;
using UnityEngine;

namespace CreateCharacter
{
    [MemoryPackable]
    [Serializable]
    public partial class ArenaManager
    {
        [field: SerializeField] public int CurrentArenaCount { get; set; }
        [field: SerializeField] public int Points { get; set; }
        [field: SerializeField] public List<RankReward> Rewards { get; set; } = new();
    }
}