using Base;
using CreateCharacter;
using MemoryPack;
using Network.Extensions;
using System;
using AllMaps.EventReward;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ResultCombatResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public RewardSnapshot Reward { get; set; } = new();
        public int NewHP { get; set; }
        public CamNgo[] NewCamNgo { get; set; } = Array.Empty<CamNgo>();
        public int NewNgoDaoDiem { get; set; }
        public ETypeResultCombat TypeResultCombat { get; set; }
        public int NewPointRank { get; set; }
        public int NewTamCanh { get; set; }
    }
}