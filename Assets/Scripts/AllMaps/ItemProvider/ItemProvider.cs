using CreateCharacter;
using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;
using UnityEngine.UI;
using Base;
using System.Collections.Generic;

namespace AllMaps.ItemProvider
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
        public IItemData IItemData { get; set; }
        public Equipment EquipmentData => ItemProviderFactory.GetProvider<Equipment>(ItemRef.Type)?.GetDataItemByIdRef(ItemRef.Id);
        public FurnaceData FurnaceData { get; set; }
        public EActionShowItemInfor ActionShowItem;
        public System.Action<int> SetPriceText;
        public BaseItemData BaseItemData { get; set; }
        public Text PriceText { get; set; }

    }
    public interface IItemProvider
    {
        ItemType SupportedType { get; }
        void Display(ToolTipContext ctx);
        int GetPrice(ToolTipContext ctx);
        int GetPriceItemSell(ushort id);

        int GetPriceItemBuy(ushort id);
        int ValueTangQuaNPCWithItem(ushort id, int npcLevel);
    }
    public interface IUseItemProvider
    {
        void UseItem(ItemInventory itemInventory);
    }
    public abstract class ItemProvider<T> : IItemProvider
    {
        public abstract T GetDataItemByIdRef(ushort Id);
        public abstract ItemType SupportedType { get; }
        public virtual void Display(ToolTipContext ctx)
        {
            ctx.SetPriceText?.Invoke(GetPrice(ctx));
        }

        public int GetPrice(ToolTipContext ctx)
        {
            return ctx.ActionShowItem == EActionShowItemInfor.None || ctx.ActionShowItem == EActionShowItemInfor.Sell
                ? GetPriceItemSell(ctx.ItemRef.Id)
                : GetPriceItemBuy(ctx.ItemRef.Id);
        }
        public abstract int GetPriceItemSell(ushort id);

        public virtual int GetPriceItemBuy(ushort id)
        {
            return GetPriceItemSell(id) * 3;
        }
        public virtual int BaseValueLevelWithTier => 0;
        public virtual int ValueTangQuaNPCWithItem(ushort id , int npcLevel)
        {
            var data = GetDataItemByIdRef(id);
            int levelWithTier = BaseValueLevelWithTier;
            if(data is IItemData itemData)
            {
                levelWithTier += GameManager.tierItemWithLevel.GetValueOrDefault(itemData.Tier);
               
            }
            else if(data is BaseSkill baseSkill)
            {
                levelWithTier += GameManager.tierSkillWithLevel.GetValueOrDefault(baseSkill.eTierSkill);

            }
            else if(data is Equipment equipment)
            {
                levelWithTier += GameManager.tierEquipmentWithLevel.GetValueOrDefault(equipment.GradeConfig.Tier);

            }
            return levelWithTier - npcLevel;
        }
             

    }

}
