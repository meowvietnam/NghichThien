using LuyenDan;

namespace AllMaps.ItemProvider
{
    public class RecipeProvider : ItemProvider<RecipeData>
    {
        public override ItemType SupportedType => ItemType.Recipe;
        public override int BaseValueLevelWithTier => 6;

        public override void Display(ToolTipContext ctx)
        {
            base.Display(ctx);

            ctx.TypeText.text = "Đan Phương";
            ctx.DescriptionText.text = $"Công thức luyện chế {ctx.IItemData.Name}";
            ctx.QualityText.text = GameManager.TextWithTier[ctx.IItemData.Tier];
        }

        public override RecipeData GetDataItemByIdRef(ushort Id)
        {
           return LuyenDanResources.Instance.FindRecipe(Id);
        }

        public override int GetPriceItemSell(ushort Id)
        {
            var data = GetDataItemByIdRef(Id);

            if (data != null && data is RecipeData recipe)
            {
                return recipe.LinhDan.PriceV130 * 5;
            }
            return 0;
        }
    }
}
