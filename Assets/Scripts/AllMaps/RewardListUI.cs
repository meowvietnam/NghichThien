using System.Collections.Generic;
using LuyenDan;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class RewardListUI : MonoBehaviour
    {
        [SerializeField] private Button claimButton;
        [SerializeField] private Button removeButton;
        [SerializeField] private GameObject rewardObjects;
        [SerializeField] private RewardUI rewardUIPrefab;
        private MailUI mailUI;
        public void Init(MailUI mailUI,List<ItemRef> rewardItem, bool claim)
        {
            this.mailUI = mailUI;
            var active = rewardItem.Count > 0;
            rewardObjects.SetActive(active);
            claimButton.gameObject.SetActive(active);
            claimButton.onClick.AddListener(OnClaim);
            claimButton.interactable = !claim;
            removeButton.onClick.AddListener(RemoveMail);
            foreach (var reward in rewardItem)
            {
                var rewardUI = Instantiate(rewardUIPrefab, transform);
                rewardUI.Init(reward, claim);
            }
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