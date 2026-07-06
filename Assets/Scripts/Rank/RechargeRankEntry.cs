using CreateCharacter;
using MemoryPack;

namespace Rank
{
    [MemoryPackable]
    public partial class RechargeRankEntry
    {
        public global::Rank.CharacterWrapper CharacterWrapper { get; private set; }
        public long TotalSpent { get; private set; }
        [MemoryPackConstructor]
        public RechargeRankEntry()
        {
        
        }
        public RechargeRankEntry(CharacterData characterData, long totalSpent)
        {
            CharacterWrapper = new global::Rank.CharacterWrapper(characterData);
            TotalSpent = totalSpent;
        }
    }
}