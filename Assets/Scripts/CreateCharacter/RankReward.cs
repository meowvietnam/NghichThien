using MemoryPack;

namespace CreateCharacter
{
    [MemoryPackable]
    public partial class RankReward
    {
        public long Id { get; set; }
        public ETypeArenaRank Type { get; set; }
        public bool IsClaim {get; set;}
    }
}