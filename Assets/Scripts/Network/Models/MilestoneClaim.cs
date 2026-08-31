using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class MilestoneClaim : IEventRequestData
    {
        public ushort Id { get; set; }
    }
}