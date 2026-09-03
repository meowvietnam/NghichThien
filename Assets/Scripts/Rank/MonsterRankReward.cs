using System.Collections.Generic;
using System.Linq;
using Base;
using CreateCharacter;
using MemoryPack;
using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine.UI;

namespace Rank
{
    [MemoryPackable]
    public partial class MonsterRankReward
    {
        public long Id { get; set; }
        public bool IsClaim { get; set; }
        public int RequirePoints { get; set; }
        public List<IEventReward> Rewards { get; set; } = new ();
        [MemoryPackIgnore]
        private Button claimButton;
        public void ShowUI(RewardUIContext context)
        {
            context.RequirePointsText.text = $"{Utils.FormatCount(context.Points)}/{Utils.FormatCount(RequirePoints)}";
            claimButton = context.ClaimButton;
            BaseBehaviour.SetEventButton(claimButton, Claim);
            var interactable = !IsClaim && context.Points >= RequirePoints;
            claimButton.interactable = interactable;
            foreach (var reward in Rewards)
            {
                reward.ShowUI(context);
            }
        }
        private async void Claim()
        {
            var client = Client.Instance;
            var reader = await client.MemRequestWait(MessageTag.ClaimMonsterKillRewardRequest, Id);
            var result = reader.MemRead<ClaimMonsterKillRewardResult>();
            if (!result.Success)
            {
                NotifyGroup.Instance.Show(result.Message);
            }
            else
            {
                Rewards = result.Rewards;
                IsClaim = true;
                var charData = client.CharacterData;
                foreach (var reward in Rewards)
                {
                    reward.Claim(charData);
                }
                var text = $"Bạn nhận được: {string.Join(", ", Rewards.Select(x => x.GetRichText()))}";
                NotifyGroup.Instance.Show(text);
                claimButton.interactable = false;
            }
        }
    }
}