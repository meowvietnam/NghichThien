using Base;
using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;

namespace Auction
{
    public class LinhDanData : IItemAuctionData
    {
        public int Id { get; }
        public Sprite Icon { get; }
        public Color Color { get; }
        public Sprite QualityBackground { get; }
        public Sprite QualityIconUp { get; }
        public string Name { get; }
        public Tier Tier { get; }
        public string Description { get; }

        public LinhDanData(int id)
        {
            Id = id;
            var linhDan = GetItemData();
            Name = linhDan.Name;
            Tier = linhDan.Tier;
            Description = linhDan.Description;
            Icon = linhDan.Icon;
            var quality = CoreConfig.Instance.FindQuality(Tier);
            Color = quality.Color;
            QualityBackground = quality.Background;
            QualityIconUp = quality.IconUp;
        }

        public void ShowInfo(ToolTipItems toolTip)
        {
            var itemRef = new ItemRef()
            {
                Id = (ushort)Id,
                Count = 1,
                Type = ItemType.LinhDan,
            };
            toolTip.ShowInfoItem(GetItemData(), itemRef);
        }

        private IItemData GetItemData()
        {
            return LuyenDanResources.Instance.FindLinhDan(Id);
        }
    }
}