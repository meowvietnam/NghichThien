using LuyenDan;

namespace ConvertDanLu
{
    /// <summary>
    /// UI slot for the result đan phương shown in ResultPage.
    /// Overrides OnShowInfo to display the recipe tooltip via ToolTipItems.
    /// </summary>
    public class ResultItemUI : BaseItemUI
    {
        protected override void OnShowInfo()
        {
            if (DanData == null) return;
            var itemRef    = DanData.ItemRef;
            var recipeData = LuyenDanResources.Instance.FindRecipe(itemRef.Id);
            if (recipeData == null) return;
            ToolTipItems.Instance.ShowInfoItem(recipeData, itemRef);
        }
    }
}
