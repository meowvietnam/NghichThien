using System.Linq;
using LuyenDan;
using Network;

namespace ConvertDanLu
{
    /// <summary>
    /// UI slot for the result đan lô (furnace) shown in ResultPage.
    /// Overrides OnShowInfo to display the furnace tooltip via ToolTipItems.
    /// </summary>
    public class ResultItemUI : BaseItemUI
    {
        protected override void OnShowInfo()
        {
            if (DanData == null) return;
            var itemRef    = DanData.ItemRef;
            // itemRef.Id = SelfId, itemRef.TempId = furnace type Id
            var tempData   = LuyenDanResources.Instance.TempFurnaceDatas
                .Find(x => x.Id == itemRef.TempId);
            if (tempData == null) return;
            var furnaceData = Client.Instance.CharacterData.InventoryManager.Furnaces
                .FirstOrDefault(f => f.SelfId == itemRef.Id);
            ToolTipItems.Instance.ShowInfoItem(tempData, itemRef, furnaceData);
        }
    }
}
