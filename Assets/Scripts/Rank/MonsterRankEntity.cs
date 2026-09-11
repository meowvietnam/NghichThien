using CreateCharacter;
using MemoryPack;

namespace Rank
{
    [MemoryPackable]
    public partial class MonsterRankEntity
    {
        public CharacterWrapper CharacterWrapper { get; set; }
        public int Point { get; set; }

        [MemoryPackConstructor]
        public MonsterRankEntity()
        {
        }

        public MonsterRankEntity(CharacterData charData, int point)
        {
            CharacterWrapper = new CharacterWrapper(charData);
            Point = point;
        }
    }
}