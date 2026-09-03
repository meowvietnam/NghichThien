using CreateCharacter;
using MemoryPack;
namespace DP
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(DongPhuRewardMoney))]
    [MemoryPackUnion(1, typeof(DongPhuRewardMoneyVip))]
    [MemoryPackUnion(2, typeof(DongPhuRewardHerb))]
    public partial interface IDongPhuReward
    {
        void Claim(CharacterData charData);
        string GetText();
    }
}