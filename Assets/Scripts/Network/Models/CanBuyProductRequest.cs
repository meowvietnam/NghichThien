using IAP;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class CanBuyProductRequest
    {
        public Store Store { get; set; }
        public string ProductId { get; set; }
    }
}