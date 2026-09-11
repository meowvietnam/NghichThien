using CreateCharacter;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class MailMonthlyPass : IMailReward
    {
        public string PassName { get; set; }
        public int MoneyVipPerDay { get; set; }

        public void ClaimReward(CharacterData characterData)
        {
        }

        public void ShowUI(MailRewardUIContext context)
        {
            context.RewardUI.SetActive(false);
        }

        public string GetRichText()
        {
            return PassName;
        }
    }
}