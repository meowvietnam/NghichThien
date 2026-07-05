using CreateCharacter;
using LuyenDan;
using TMPro;
using UnityEngine;

namespace AllMaps
{
    public class RewardUI : LuyenDan.BaseItemUI
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject claimed;
        public void Init(IMailReward rewardItem, bool claim)
        {
            SetClaim(claim);
            var context = new MailRewardUIContext()
            {
                RewardUI = this
            };
            rewardItem.ShowUI(context);
        }

        public void SetClaim(bool claim)
        {
            claimed.SetActive(claim);
            canvasGroup.alpha = claim ? 0.65f : 1f;
        }

        public void DestroyObject()
        {
            Destroy(selfObject);
        }
    }
}