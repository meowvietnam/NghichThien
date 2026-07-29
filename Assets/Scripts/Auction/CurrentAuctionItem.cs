using Base;
using LuyenDan;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class CurrentAuctionItem : MonoBehaviour
    {
        [SerializeField] private ItemAuction item;
        [SerializeField] private Text currentPriceTitle;
        [SerializeField] private Text currentPrice;
        [SerializeField] private Text minPrice;
        [SerializeField] private Text maxPrice;
        public int MinPrice { get; private set; }
        public int MaxPrice { get; private set; }
        public int CurrentPrice { get; private set; }
        public void SetItem(ItemRef itemRef)
        {
            var item = ItemFactory.CreateItem(itemRef);
            this.item.SetItem(item);
        }
        public void SetMinPrice(int minPrice)
        {
            MinPrice = minPrice;
            this.minPrice.text = Utils.FormatCount(minPrice);
        }
        public void SetMaxPrice(int maxPrice)
        {
            MaxPrice = maxPrice;
            this.maxPrice.text = Utils.FormatCount(maxPrice);
        }
        public void SetCurrentPrice(int currentPrice)
        {
            CurrentPrice = currentPrice;
            this.currentPrice.text = Utils.FormatCount(currentPrice);
        }
        public void SetCurrentPlayerBid(string playerName)
        {
            var baseText = "Giá hiện tại";
            if (string.IsNullOrEmpty(playerName))
            {
                currentPriceTitle.text = baseText;
            }
            else
            {
                currentPriceTitle.text = $"{baseText} [<color=yellow>{playerName}</color>]";
            }
        }
    }
}
