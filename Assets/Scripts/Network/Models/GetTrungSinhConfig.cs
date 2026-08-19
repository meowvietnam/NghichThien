using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class GetTrungSinhConfig : Result
    {
        public TrungSinhConfig Config { get; set; }
    }
}