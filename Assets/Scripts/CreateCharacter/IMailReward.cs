
using AllMaps;
using MemoryPack;
using UnityEngine;

public class MailRewardUIContext
{
    public RewardUI RewardUI { get; set; }
}
namespace CreateCharacter
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(MailRewardItem))]
    [MemoryPackUnion(1, typeof(MailRewardFurnace))]
    [MemoryPackUnion(2, typeof(MailRewardMoney))]
    [MemoryPackUnion(3, typeof(MailRewardMoneyVip))]
    public partial interface IMailReward
    {
        void ClaimReward(CharacterData characterData);
        void ShowUI(MailRewardUIContext context);
        string GetRichText();
    }
}