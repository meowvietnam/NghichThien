using CreateCharacter;
using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;
using UnityEngine.UI;
using Base;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class ToolTipContext
    {
        public Text TypeText { get; set; }
        public Text QualityText { get; set; }
        public Text DescriptionText { get; set; }
        public Text DescTextItem { get; set; }
        public Image PrefabIconRequire { get; set; }
        public Image PrefabLeftInfor { get; set; }
        public Image PrefabRightInfor { get; set; }
        public Sprite[] SpriteLinhKhis { get; set; }
        public ItemRef ItemRef { get; set; }
        public DataItemSnapshot ItemSnapshot { get; set; }
        public IItemData IItemData => ItemSnapshot.IItemData;
        public Equipment EquipmentData => ItemSnapshot.EquipmentData;
        public FurnaceData FurnaceData => ItemSnapshot.FurnaceData;
        public EActionShowItemInfor ActionShowItem;
        public System.Action<int> SetPriceText;
        public BaseItemData BaseItemData => ItemSnapshot.BaseItemData;

    }
 
    public abstract class ItemDisplayer
    {
        public abstract ItemType SupportedType { get; }
        public abstract void Display(ToolTipContext ctx);

        public int GetPrice(ToolTipContext ctx)
        {
            return ctx.ActionShowItem == EActionShowItemInfor.None || ctx.ActionShowItem == EActionShowItemInfor.Sell
                ? GetPriceItemSell(ctx.ItemSnapshot)
                : GetPriceItemBuy(ctx.ItemSnapshot);
        }
        public abstract int GetPriceItemSell(DataItemSnapshot ctx);
       

        public virtual int GetPriceItemBuy(DataItemSnapshot ctx)
        {
            return GetPriceItemSell(ctx) * 3;
        }

    }

}
