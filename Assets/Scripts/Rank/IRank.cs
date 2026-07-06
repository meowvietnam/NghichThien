using System;
using AllMaps.ui_rank;
using MemoryPack;
using RankType = AllMaps.ui_rank.RankType;

namespace Rank
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(RankLevel))]
    [MemoryPackUnion(1, typeof(RankTongNap))]
    [MemoryPackUnion(2, typeof(RankTongTieu))]
    [MemoryPackUnion(3, typeof(RankArena))]
    public partial interface IRank
    {
        RankType Type { get; }
        TimeSpan NextResetTime { get; }
        void Init(RankPanel panel);
    }
}