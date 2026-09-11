using Base;
using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;

namespace Auction
{
    public class RecipeData : IItemAuctionData
    {
        public int Id { get; }
        public string Name { get; }
        public Sprite Icon { get; }
        public Color Color { get; }
        public Sprite QualityBackground { get; }
        public Sprite QualityIconUp { get; }

        public RecipeData(int id)
        {
            Id = id;
            var item = GetItemData();
            Name = item.Name;
            Icon = item.Icon;
            var quality = CoreConfig.Instance.FindQuality(item.Tier);
            Color = quality.Color;
            QualityBackground = quality.Background;
            QualityIconUp = quality.IconUp;
        }

        public IItemData GetItemData()
        {
            return LuyenDanResources.Instance.FindRecipe((ushort)Id);
        }

        public void ShowInfo(ToolTipItems toolTip)
        {
            var item = GetItemData();
            var itemRef = new ItemRef()
            {
                Id = (ushort)Id,
                Type = ItemType.Recipe,
                Count = 1
            };
            toolTip.ShowInfoItem(item, itemRef);
        }
    }
}