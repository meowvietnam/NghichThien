using CreateCharacter;
using MemoryPack;

namespace Rank
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(MoneyVipMonsterRankReward))]
    [MemoryPackUnion(1, typeof(ItemMonsterRankReward))]
    [MemoryPackUnion(2, typeof(LinhDanMonsterRankReward))]
    public partial interface IMonsterRankReward
    {
        void Claim(CharacterData charData);
        void ShowUI(MonsterRankRewardUIContext context);
        string GetRichText();
    }
}