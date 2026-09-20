using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class SyncSendEndTurnBoss
    {
        public int CharId { get; set; }
        public bool IsKietSuc { get; set; }
    }
}