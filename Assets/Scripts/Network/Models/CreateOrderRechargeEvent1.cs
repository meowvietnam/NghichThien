using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class CreateOrderRechargeEvent1 : IEventRequestData
    {
        public ushort Id { get; set; }
    }
}