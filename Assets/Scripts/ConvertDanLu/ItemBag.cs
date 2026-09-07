using System.Linq;
using LuyenDan;
using Network;
using UnityEngine.EventSystems;

namespace ConvertDanLu
{
    /// <summary>
    /// A clickable đan lô item shown inside the Bag scroll view.
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
            var itemRef     = DanData.ItemRef;
            var itemData    = DanData.ItemData;
            var furnaceData = Client.Instance.CharacterData.InventoryManager.Furnaces
                .FirstOrDefault(f => f.SelfId == itemRef.Id);

            // Reuse IItemData via LuyenDanResources for the tooltip
            var tempData = LuyenDanResources.Instance.TempFurnaceDatas
                .Find(x => x.Id == (furnaceData?.Id ?? 0));
            if (tempData == null) return;

            ToolTipItems.Instance.ShowInfoItem(tempData, itemRef, furnaceData);
        }
    }
}
