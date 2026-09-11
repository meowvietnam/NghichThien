using LuyenDan;
using UnityEngine.EventSystems;

namespace ConvertDanLu
{
    /// <summary>
    /// A clickable đan phương (recipe) item shown inside the Bag scroll view.
    /// When clicked it notifies the Bag, which forwards to the waiting Slot callback.
    /// </summary>
    public class ItemBag : BaseItemUI, IPointerEnterHandler, IPointerExitHandler
    {
        protected override void OnSelected()
        {
            ConvertDanLuPanel.Instance.Bag.OnSelected(this);
        }

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
