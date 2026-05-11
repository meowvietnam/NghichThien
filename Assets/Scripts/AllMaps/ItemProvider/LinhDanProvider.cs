using LuyenDan;
using LuyenDan.Inventory;

namespace AllMaps.ItemProvider
{
    public class LinhDanProvider : ItemProvider<LinhDan>
    {
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

        public override LinhDan GetDataItemById(ushort Id)
        {
            return LuyenDanResources.Instance.FindLinhDan(Id);

        }

        public override int GetPriceItemSell(ushort Id)
        {
            var data = GetDataItemById(Id);
            if (data != null && data is LinhDan linhDan)
            {
                return linhDan.PriceV130;
            }
            return 0;
        }
    }
}
