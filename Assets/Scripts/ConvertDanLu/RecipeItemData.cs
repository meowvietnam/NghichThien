using Base;
using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;

namespace ConvertDanLu
{
    /// <summary>
    /// Wraps a RecipeData (đan phương) ItemRef as an IDanItemData for the result page UI.
    /// </summary>
    public class RecipeItemData : IDanItemData
    {
        private readonly RecipeData recipeData;
        private readonly DanVisualQuality visualQuality;

        public RecipeItemData(ItemRef itemRef)
        {
            recipeData = LuyenDanResources.Instance.FindRecipe(itemRef.Id);

            var quality = CoreConfig.Instance.FindQuality(recipeData.Tier);
            visualQuality = new DanVisualQuality(quality.Background, quality.IconUp, quality.Color);
        }

        public int     Id          => recipeData.Id;
        public Sprite  Icon        => recipeData.Icon;
        public string  Name        => recipeData.Name;
        public Tier    Tier        => recipeData.Tier;
        public string  Description => recipeData.Description;
        public DanVisualQuality VisualQuality => visualQuality;
    }
}
