using LuyenDan;
using TMPro;
using UnityEngine;

namespace AllMaps
{
    public class RewardUI : LuyenDan.BaseItemUI
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject claimed;
        public void Init(ItemRef rewardItem, bool claim)
        {
            SetClaim(claim);
            var data = LuyenDanResources.Instance.FindItem(rewardItem);
            SetData(data);
        }

        public void SetClaim(bool claim)
        {
            claimed.SetActive(claim);
            canvasGroup.alpha = claim ? 0.65f : 1f;
        }
    }
}