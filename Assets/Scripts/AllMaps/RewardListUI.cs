using System.Collections.Generic;
using System.Security.Claims;
using CreateCharacter;
using LuyenDan;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class RewardListUI : BaseBehaviour
    {
        [SerializeField] private Button claimButton;
        [SerializeField] private Button removeButton;
        [SerializeField] private GameObject rewardObjects;
        [SerializeField] private RewardUI rewardUIPrefab;
        private List<RewardUI> rewardItems = new();
        private MailUI mailUI;

        public void Init(MailUI mailUI, List<IMailReward> rewardItem, bool claim)
        {
            this.mailUI = mailUI;
            var active = rewardItem.Count > 0;
            rewardObjects.SetActive(active);
            SetEventButton(removeButton, RemoveMail);
            SetEventButton(claimButton, OnClaim);
            claimButton.gameObject.SetActive(active);
            claimButton.interactable = !claim;
            var needed = rewardItem.Count - rewardItems.Count;
            for (int i = 0; i < needed; i++)
            {
                var rewardUI = Instantiate(rewardUIPrefab, transform);
                rewardItems.Add(rewardUI);
            }

            if (needed < 0)
            {
                for (int i = rewardItems.Count + needed; i < rewardItems.Count; i++)
                {
                    var rewardUI = rewardItems[i];
                    rewardUI.SetActive(false);
                }
            }

            for (var i = 0; i < rewardItem.Count; i++)
            {
                var reward = rewardItem[i];
                var rewardUI = rewardItems[i];
                rewardUI.SetActive(true);
                rewardUI.Init(reward, claim);
            }
        }

        public void SetClaim(bool claim)
        {
            claimButton.interactable = !claim;
            rewardItems.ForEach(reward => reward.SetClaim(claim));
        }

        private void RemoveMail()
        {
            mailUI.RemoveMail();
        }

        private void OnClaim()
        {
            mailUI.OnClaim();
        }

        private void OnDestroy()
        {
            claimButton.onClick.RemoveAllListeners();
            removeButton.onClick.RemoveAllListeners();
        }
    }
}