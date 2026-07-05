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
        public void Init(MailUI mailUI,List<IMailReward> rewardItem, bool claim)
        {
            DestroyAll();
            this.mailUI = mailUI;
            var active = rewardItem.Count > 0;
            rewardObjects.SetActive(active);
            SetEventButton(removeButton,RemoveMail);
            SetEventButton(claimButton,OnClaim);
            claimButton.gameObject.SetActive(active);
            claimButton.interactable = !claim;
            foreach (var reward in rewardItem)
            {
                var rewardUI = Instantiate(rewardUIPrefab, transform);
                rewardUI.Init(reward, claim);
                rewardItems.Add(rewardUI);
            }
        }

        private void DestroyAll()
        {
            claimButton.onClick.RemoveAllListeners();
            removeButton.onClick.RemoveAllListeners();
            foreach (var rewardItem in rewardItems)
            {
                rewardItem.DestroyObject();
            }
            rewardItems.Clear();
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