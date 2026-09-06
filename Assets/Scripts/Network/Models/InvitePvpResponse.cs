using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class InvitePvpResponse
    {
        public bool Accept { get; set; }
    }
}