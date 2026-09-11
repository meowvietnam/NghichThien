using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class SyncEndTurn
    {
        public int CharId { get; set; }
    }
}