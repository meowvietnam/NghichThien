using LuyenDan;
using LuyenDan.Inventory;
using Network;
using System.Linq;
using CreateCharacter;
using LongDHModules;

namespace AllMaps.ItemProvider
{
    public class RecipeProvider : ItemProvider<RecipeData>, IUseItemProvider
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
                ctx.DescriptionText.text += $"{GetColoredText("[Hiệu quả]", "#ffb143")}: {recipe.LinhDan.Description}";

                for (int i = 0; i < recipe.Recipes.Count; i++)
                {
                    var r = recipe.Recipes[i];

                    // Thêm tiêu đề công thức
                    ctx.DescriptionText.text += $"\n{GetColoredText($"[Công thức {i + 1}]", "#ffb143")}\n";

                    // Lấy danh sách các dòng thảo dược
                    string herbLines = "";
                    herbLines += GetHerbString("Chủ dược", r.ChuDuoc);
                    herbLines += GetHerbString("Phụ dược 1", r.PhuDuoc1);
                    herbLines += GetHerbString("Phụ dược 2", r.PhuDuoc2);
                    herbLines += GetHerbString("Dược dẫn", r.DuocDan);

                    // Xóa ký tự xuống dòng thừa ở cuối nếu có (nếu GetHerbString có trả về)
                    ctx.DescriptionText.text += herbLines.TrimEnd('\n');
                }
                ctx.DescriptionText.text += "\n [Dược liệu nhị phẩm trở xuống có thể mua tại Vân Tịch Thành, còn dược liệu tam phẩm trở lên đánh quái có tỉ lệ rớt ra, làm sự kiện hoặc quay thưởng]";
            }

            ctx.QualityText.text = GameManager.TextWithTier[ctx.IItemData.Tier];
        }

        // Sửa lại hàm này để không tự thêm \n ở cuối
        string GetHerbString(string label, RecipeData.Herbs herb)
        {
            if (herb == null || herb.Id <= 0) return "";

            var herbData = LuyenDanResources.Instance.FindHerb(herb.Id);
            string name = herbData != null ? herbData.Name : "Unknown";

            return $"{label}: {name} x{herb.Count} ({GameManager.TextWithTier[herbData.Tier]})\n";
        }

        string GetColoredText(string text, string color)
        {
            return $"<color={color}>{text}</color>";
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

        public void UseItem(ItemInventory itemInventory)
        {
            NotifyGroup.Instance?.Show("Sử dụng để luyện đan tại Phòng luyên đan!");
        }
    }
}