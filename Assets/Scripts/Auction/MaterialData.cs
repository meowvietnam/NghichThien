using Base;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using UnityEngine;

namespace Auction
{
    public class MaterialData : IItemAuctionData
    {
        public int Id { get; }
        public Sprite Icon { get; }
        public Color Color { get; }
        public Sprite QualityBackground { get; }
        public Sprite QualityIconUp { get; }
        public string Name { get; }
        public Tier Tier { get; }
        public string Description { get; }

        public MaterialData(int id)
        {
            Id = id;
            var material = GetItemData();
            Icon = material.Icon;
            var quality = CoreConfig.Instance.FindQuality(material.Tier);
            Color = quality.Color;
            QualityBackground = quality.Background;
            QualityIconUp = quality.IconUp;
            Name = material.Name;
            Tier = material.Tier;
            Description = material.Description;
        }

        public void ShowInfo(ToolTipItems toolTip)
        {
            var itemRef = new ItemRef()
            {
                Id = (ushort)Id,
                Count = 1,
                Type = ItemType.Material,
            };
            toolTip.ShowInfoItem(GetItemData(), itemRef);
        }

        public IItemData GetItemData()
        {
            return LuyenKhiResources.Instance.FindMaterial((ushort)Id);
        }
    }
}