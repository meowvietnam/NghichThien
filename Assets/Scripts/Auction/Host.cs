using System;
using Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class Host : MonoBehaviour
    {
        [SerializeField] private SayDialog sayDialog;
        [SerializeField] private Text endAfterText;
        [SerializeField] private Image progressBar;
        private TimeSpan auctionDuration = TimeSpan.FromSeconds(160);
        private TimeSpan endAfter = TimeSpan.FromSeconds(155);

        private void FixedUpdate()
        {
            var deltaTime = TimeSpan.FromSeconds(Time.fixedUnscaledDeltaTime);
            SetEndAfter(endAfter - deltaTime);
        }

        public void SetAuctionDuration(TimeSpan auctionDuration)
        {
            this.auctionDuration = auctionDuration;
        }

        public void SetEndAfter(TimeSpan endAfter)
        {
            if (this.endAfter == endAfter)
            {
                return;
            }

            this.endAfter = endAfter > TimeSpan.Zero ? endAfter : TimeSpan.Zero;
            endAfterText.text = $"Phiên đấu giá kết thúc sau {Utils.GetTime(this.endAfter)}";
            if (auctionDuration == TimeSpan.Zero)
            {
                progressBar.fillAmount = 0;
            }
            else
            {
                progressBar.fillAmount = this.endAfter.Ticks * 1f / auctionDuration.Ticks;
            }
        }

        public void Chat(string message, float duration)
        {
            sayDialog.Chat(message, duration);
        }
    }
}