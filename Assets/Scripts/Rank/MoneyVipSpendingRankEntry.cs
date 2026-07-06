using CreateCharacter;
using MemoryPack;

namespace Rank
{
    [MemoryPackable]
    public partial class MoneyVipSpendingRankEntry
    {
        public global::Rank.CharacterWrapper CharacterWrapper { get; private set; }
        public long TotalSpent { get; private set; }
        [MemoryPackConstructor]
        public MoneyVipSpendingRankEntry()
        {
        
        }
        public MoneyVipSpendingRankEntry(CharacterData characterData, long totalSpent)
        {
            CharacterWrapper = new global::Rank.CharacterWrapper(characterData);
            TotalSpent = totalSpent;
        }
    }
}