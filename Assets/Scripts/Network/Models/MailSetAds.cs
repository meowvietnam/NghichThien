using CreateCharacter;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class MailSetAds : IMailReward
    {
        public bool NoAds { get; set; }
        public void ClaimReward(CharacterData characterData)
        {
            characterData.IsNoAds = NoAds;
        }

        public void ShowUI(MailRewardUIContext context)
        {
            context.RewardUI.SetActive(false);
        }

        public string GetRichText()
        {
            return NoAds ? "Bỏ quảng cáo" : $"Mở quảng cáo";
        }
    }
}