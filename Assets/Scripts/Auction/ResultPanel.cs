using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class ResultPanel : BasePanel
    {
        [SerializeField] private Text contentText;
        [SerializeField] private ItemAuction itemAuction;

        public void SetData(AuctionReward reward)
        {
            contentText.text = reward.Content;
            var item = ItemFactory.CreateItem(reward.Item);
            itemAuction.SetItem(item);
        }
    }
}