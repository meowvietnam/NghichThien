
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
        public RewardItems RewardItems { get; set; }
        public int NewTienNgoc { get; set; }
        public int Money { get; set; }
        public int MoneyVip { get; set; }
    }
}