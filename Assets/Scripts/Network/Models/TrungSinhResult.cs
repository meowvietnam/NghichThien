using MemoryPack;
using Rank;

namespace Network.Models
{
    [MemoryPackable]
    public partial class TrungSinhResult : Result
    {
        public CharacterWrapper Wrapper { get; set; }
    }
}