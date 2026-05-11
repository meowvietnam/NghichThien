using LuyenDan;

namespace AllMaps.TabPanel.ToolTipDisplayers
{
    public class RecipeDisplayer : ItemDisplayer
    {
        public override ItemType SupportedType => ItemType.Recipe;

        public override void Display(ToolTipContext ctx)
        {
            ctx.TypeText.text = "Đan Phương";
            ctx.DescriptionText.text = $"Công thức luyện chế {ctx.IItemData.Name}";
            ctx.QualityText.text = GameManager.TextWithTier[ctx.IItemData.Tier];
            ctx.SetPriceText?.Invoke(GetPrice(ctx));
        }

        public override int GetPriceItemSell(DataItemSnapshot ctx)
        {
            var data = ctx.IItemData;

            if (data != null && data is RecipeData recipe)
            {
                return recipe.LinhDan.PriceV130 * 5;
            }
            return 0;
        }
    }
}
