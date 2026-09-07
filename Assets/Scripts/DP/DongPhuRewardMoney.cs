using Base;
using CreateCharacter;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class DongPhuRewardMoney : IDongPhuReward
    {
        public int Money { get; set; }

        public DongPhuRewardMoney(int money)
        {
            Money = money;
        }

        public void Claim(CharacterData charData)
        {
            var newMoney = charData.Stats.LinhThach + Money;
            charData.Stats.LinhThach = newMoney;
        }

        public string GetText()
        {
            return $"{Utils.FormatCount(Money)} Linh thạch";
        }
    }
}