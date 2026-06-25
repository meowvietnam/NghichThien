using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using System.Linq;
using LongDHModules;

namespace AllMaps.ItemProvider
{
    public class LinhDanProvider : ItemProvider<LinhDan> , IUseItemProvider
    {
        public override int BaseValueLevelWithTier => 6;
        public override ItemType SupportedType => ItemType.LinhDan;
        
        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);

            string text = "Đan Dược";

            if (ctx.IItemData is LinhDan linhDan)
            {
                ctx.DescTextItem.transform.parent.gameObject.SetActive(true);

                switch (linhDan.ETypeDan)
                {
                    case ETypeDan.DanCombat:  text += " - Chiến Đấu"; break;
                    case ETypeDan.DanBuff:    text += " - Tăng Chỉ Số"; break;
                    case ETypeDan.DanDotPha:  text += " - Đột Phá"; break;
                    case ETypeDan.DanOther:   text += " - Khác"; break;
                }

                ctx.DescTextItem.text = $"{BaseSkill.GetColoredText("【Đan Độc】", "#d36cd3")} {linhDan.DanDoc}";
                ctx.DescriptionText.text = linhDan.Description;
            }

            ctx.TypeText.text = text;
            ctx.QualityText.text = GameManager.TextWithTier[ctx.IItemData.Tier];
        }

        public override LinhDan GetDataItemByIdRef(ushort Id)
        {
            return LuyenDanResources.Instance.FindLinhDan(Id);

        }

        public override int GetPriceItemSell(ushort Id)
        {
            var data = GetDataItemByIdRef(Id);
            if (data != null && data is LinhDan linhDan)
            {
                return linhDan.PriceV130;
            }
            return 0;
        }

        public void UseItem(ItemInventory itemInventory)
        {
            if (itemInventory.iItemData is LinhDan danDuoc)
            {
                if (danDuoc.ETypeDan == ETypeDan.DanBuff || danDuoc.ETypeDan == ETypeDan.DanCombat)
                {
                    var selectNum = Pool.Get<TabPanel>().selectNum;
                    selectNum.SetMin(0);
                    int maxValue = 0;
                    if (danDuoc.ETypeDan == ETypeDan.DanBuff)
                    {
                        maxValue = itemInventory.itemRef.Count;
                    }
                    else
                    {
                        maxValue = itemInventory.itemRef.Count <= Client.Instance.CharacterData.Progression.Level ? itemInventory.itemRef.Count : Client.Instance.CharacterData.Progression.Level; ///
                        if (maxValue <= 0)
                        {
                            NotifyGroup.Instance.Show($"Đã đạt tốt đa số lần sử dụng {itemInventory.iItemData.Name} hãy chiến đấu để làm mới");
                            return;
                        }
                    }
                    selectNum.Show(0.15f);

                    selectNum.SetMax(maxValue);
                    selectNum.ValueChanged += (value) =>
                    {
                        selectNum.SetText($"{itemInventory.iItemData.Name} x{value}");
                    };
                    selectNum.confirmAction.RemoveAllListeners();
                    selectNum.confirmAction.AddListener(() =>
                    {
                        var value = selectNum.GetValue();
                        if (value >= 1)
                        {
                            Observer.Notify(EventList.Network.USE_DAN, this, itemInventory.itemRef.Id, value);
                            itemInventory.AddCount(-value);
                        }

                    });
                }


            }
        }
    }
}
