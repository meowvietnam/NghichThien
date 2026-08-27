using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class HarvestRequest
    {
        public ushort SlotIndex { get; set; }
    }
}