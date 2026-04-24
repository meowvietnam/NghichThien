using LuyenDan;
using LuyenDan.Inventory;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class LinhDanDisplayer : IToolTipDisplayer
    {
        public ItemType SupportedType => ItemType.LinhDan;

        public void Display(ToolTipContext ctx)
        {
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
            ctx.SetPriceText?.Invoke(GetPrice(ctx));
        }

        private int GetPrice(ToolTipContext ctx)
        {
            return ctx.ActionShowItem == EActionShowItemInfor.None || ctx.ActionShowItem == EActionShowItemInfor.Sell
                ? GameManager.GetPriceItemSell(ctx.ItemRef, ctx.IItemData)
                : GameManager.GetPriceItemBuy(ctx.ItemRef, ctx.IItemData);
        }
    }
}
