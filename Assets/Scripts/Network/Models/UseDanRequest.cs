using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial struct UseDanRequest
    {
        public ushort Id { get; set; }
        public ushort Count { get; set; }
    }
}