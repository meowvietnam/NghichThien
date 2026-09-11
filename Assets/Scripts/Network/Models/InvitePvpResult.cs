using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class InvitePvpResult : Result
    {
        public InvitePvp InvitePvp { get; set; }
    }
}