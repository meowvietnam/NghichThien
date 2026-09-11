using Network.Extensions;
using Network.Extensions.MemoryPack;
using AllMaps.EventReward;
using IAP;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class IapResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Store Store { get; set; }
        public string ProductId { get; set; }
        public IIapReward Reward { get; set; }
    }
}