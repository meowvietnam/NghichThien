using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class InvitePvpRequest
    {
        public int Challenger { get; set; }
    }
}