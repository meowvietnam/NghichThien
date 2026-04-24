using LuyenDan;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class RecipeDisplayer : IToolTipDisplayer
    {
        public ItemType SupportedType => ItemType.Recipe;

        public void Display(ToolTipContext ctx)
        {
            ctx.TypeText.text = "Đan Phương";
            ctx.DescriptionText.text = $"Công thức luyện chế {ctx.IItemData.Name}";
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
