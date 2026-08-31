using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class HandleEventRequest
    {
        public ushort EventId { get; set; }
        public IEventRequestData Data { get; set; }
    }
}