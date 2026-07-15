using LuyenDan;
using LuyenDan.Inventory;
using Network;
using System.Linq;

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
            ctx.DescriptionText.text = $"Công thức luyện chế {ctx.IItemData.Name}\n";

            if (ctx.IItemData is RecipeData recipe)
            {
                // Hiển thị [Hiệu quả] với màu #ffb143
                ctx.DescriptionText.text += $"{GetColoredText("[Hiệu quả]", "#ffb143")}: {recipe.LinhDan.Description}\n";

                for (int i = 0; i < recipe.Recipes.Count; i++)
                {
                    var r = recipe.Recipes[i];

                    // Hiển thị [Công thức i+1] với màu #ffb143
                    ctx.DescriptionText.text += $"{GetColoredText($"[Công thức {i + 1}]", "#ffb143")}\n";

                    ctx.DescriptionText.text += GetHerbString("Chủ dược", r.ChuDuoc);
                    ctx.DescriptionText.text += GetHerbString("Phụ dược 1", r.PhuDuoc1);
                    ctx.DescriptionText.text += GetHerbString("Phụ dược 2", r.PhuDuoc2);
                    ctx.DescriptionText.text += GetHerbString("Dược dẫn", r.DuocDan);

                    ctx.DescriptionText.text += "\n";
                }
            }

            // Giả sử hàm GetColoredText của bạn trông như thế này:
            string GetColoredText(string text, string color)
            {
                return $"<color={color}>{text}</color>";
            }

            string GetHerbString(string label, RecipeData.Herbs herb)
            {
                if (herb == null || herb.Id <= 0) return "";

                var herbData = LuyenDanResources.Instance.FindHerb(herb.Id);
                string name = herbData != null ? herbData.Name : "Unknown";

                return $"{label}: {name} x{herb.Count}\n";
            }
            ctx.QualityText.text = GameManager.TextWithTier[ctx.IItemData.Tier];
        }

        public override RecipeData GetDataItemByIdRef(ushort Id)
        {
           return LuyenDanResources.Instance.FindRecipe(Id);
        }
        public override int GetCountOwner(ushort id)
        {
            var item = Client.Instance.CharacterData.InventoryManager.Recipes.FirstOrDefault(x => x.Id == id);
            return item != null ? item.Count : base.GetCountOwner(id);
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
