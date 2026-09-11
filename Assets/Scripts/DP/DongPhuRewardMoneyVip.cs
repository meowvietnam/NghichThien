using Base;
using CreateCharacter;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class DongPhuRewardMoneyVip : IDongPhuReward
    {
        public int MoneyVip { get; set; }

        public void Claim(CharacterData charData)
        {
            var newMoneyVip = charData.MoneyVip + MoneyVip;
            charData.MoneyVip = newMoneyVip;
        }

        public string GetText()
        {
            return $"{Utils.FormatCount(MoneyVip)} Tiên ngọc";
        }
    }
}