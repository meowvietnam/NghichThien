using CreateCharacter;
using MemoryPack;
using Rank;

namespace Network.Models
{
    [MemoryPackable]
    public partial class SyncCharacterDataResult : Result
    {
        public CharacterWrapper Character { get; set; }
    }
}