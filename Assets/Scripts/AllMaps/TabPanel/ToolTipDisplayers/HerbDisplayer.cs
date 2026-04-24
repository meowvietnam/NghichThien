using LuyenDan;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class HerbDisplayer : IToolTipDisplayer
    {
        public ItemType SupportedType => ItemType.Herb;

        public void Display(ToolTipContext ctx)
        {
            ctx.TypeText.text = "Dược Liệu";
            ctx.DescriptionText.text = "Thảo dược có thể dùng luyện chế đan dược";
            if (ctx.IItemData is HerbData duocLieu)
            {
                ctx.DescTextItem.transform.parent.gameObject.SetActive(true);
                ctx.DescTextItem.text =
                    "<color=#ffb143>【Chủ Dược】 </color>" + $"{duocLieu.ChuDuoc} " +
                    "<color=#ffb143>【Phụ Dược】 </color>" + $"{duocLieu.PhuDuoc} " +
                    "<color=#ffb143>【Dược Dẫn】 </color>" + $"{duocLieu.ThuocDan}";
            }
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
