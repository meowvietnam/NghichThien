using UnityEngine;

namespace Auction
{
    public interface IItemAuctionData
    {
        public string Name { get; }
        public Sprite Icon { get; }
        public Color Color { get; }
        public Sprite QualityBackground { get; }
        public Sprite QualityIconUp { get; }
        void ShowInfo(ToolTipItems toolTip);
    }
}