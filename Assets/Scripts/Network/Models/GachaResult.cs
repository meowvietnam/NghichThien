
using Network.Extensions;
using Network.Extensions.MemoryPack;
using AllMaps.EventReward;
using IAP;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class GachaResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public RewardSnapshot Reward { get; set; }
        public int NewTienNgoc { get; set; }

    }
}