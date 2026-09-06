using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class PvpMatched
    {
        public CharacterSnapshot Inviter { get; set; }
        public CharacterSnapshot Defender { get; set; }
    }
}