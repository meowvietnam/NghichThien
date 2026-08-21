using CreateCharacter;
using MemoryPack;

namespace Rank
{
    [MemoryPackable]
    public partial class MoneyVipMonsterRankReward : IMonsterRankReward
    {
        public int MoneyVip { get; set; }
        public void Claim(CharacterData charData)
        {
            var newMoneyVip = charData.MoneyVip + MoneyVip;
            charData.MoneyVip = newMoneyVip;
        }

    }
}