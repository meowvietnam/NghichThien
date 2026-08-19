using Base;
using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;

namespace Auction
{
    public class FurnaceData : IItemAuctionData
    {
        public string Name { get; }
        public Tier Tier { get; }
        public string Description { get; }
        public int Id { get; }
        public Sprite Icon { get; }
        public Color Color { get; }
        public Sprite QualityBackground { get; }
        public Sprite QualityIconUp { get; }
        public FurnaceData(int id)
        {
            Id = id;
            var furnace = GetItemData();
            Name = furnace.Name;
            Tier = furnace.Tier;
            Description = furnace.Description;
            Icon = furnace.Icon;
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
                Type = ItemType.Furnace,
            };
            toolTip.ShowInfoItem(GetItemData(), itemRef);
        }

        public IItemData GetItemData()
        {
            return LuyenDanResources.Instance.FindFurnace(Id);
        }
    }
}