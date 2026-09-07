using CreateCharacter;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class MailSetInfinityEnergy : IMailReward
    {
        public bool IsInfinityEnergy { get; set; }

        public void ClaimReward(CharacterData characterData)
        {
            characterData.IsInfiniteEnergy = IsInfinityEnergy;
        }

        public void ShowUI(MailRewardUIContext context)
        {
            context.RewardUI.SetActive(false);
        }

        public string GetRichText()
        {
            return IsInfinityEnergy ? "Vô hạn năng lượng" : "Giới hạn năng lượng";
        }
    }
}