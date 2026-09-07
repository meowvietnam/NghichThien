using System;
using System.Linq;
using Base;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class HistoryCell : MonoBehaviour
    {
        [SerializeField] private AuctionSessionStatusResource statusResource;
        [SerializeField] private Text sessionTitleText;
        [SerializeField] private Text sessionTimeText;
        [SerializeField] private ItemAuction item;
        [SerializeField] private CanvasGroup itemCanvasGroup;
        [SerializeField] private Text statusText;
        [SerializeField] private HighestBidder highestBidder;

        public void Init(AuctionSession session)
        {
            SetSessionTitle(session.SessionTitle);
            SetSessionTime(session.StartTime, session.EndTime);
            item.SetItem(ItemFactory.CreateItem(session.ItemRef));
            SetStatus(session.Status);
            var player = session.HighestBidder;
            if (player != null)
            {
                highestBidder.SetName(player.Name);
                highestBidder.SetPrice(player.Amount);
                highestBidder.SetActive(true);
            }
            else
            {
                highestBidder.SetActive(false);
            }
        }

        public void SetStatus(AuctionSessionStatus status)
        {
            var data = statusResource.Datas.First(x => x.Status == status);
            statusText.text = data.Name;
            statusText.color = data.Color;
            itemCanvasGroup.alpha = data.Alpha;
        }

        public void SetSessionTitle(string title)
        {
            sessionTitleText.text = title;
        }

        public void SetSessionTime(TimeSpan start, TimeSpan end)
        {
            var text = $"Diễn ra trong: <color=gray>{Utils.FormatTimeOnly(start)}-{Utils.FormatTimeOnly(end)}</color>";
            sessionTimeText.text = text;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}