using Base;
using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;

namespace ConvertDanLu
{
    /// <summary>
    /// Wraps a FurnaceData entry as an IDanItemData for the ConvertDanLu UI.
    /// </summary>
    public class FurnaceItemData : IDanItemData
    {
        private readonly FurnaceData furnaceData;
        private readonly TempFurnaceData tempData;
        private readonly DanVisualQuality visualQuality;

        public FurnaceItemData(FurnaceData furnace)
        {
            furnaceData = furnace;
            tempData = LuyenDanResources.Instance.TempFurnaceDatas
                           .Find(x => x.Id == furnace.Id) 
                       ?? LuyenDanResources.Instance.TempFurnaceDatas[0];

            var quality = CoreConfig.Instance.FindQuality(tempData.Tier);
            visualQuality = new DanVisualQuality(quality.Background, quality.IconUp, quality.Color);
        }

        public int     Id          => furnaceData.SelfId;
        public Sprite  Icon        => tempData.Icon;
        public string  Name        => tempData.Name;
        public Tier    Tier        => tempData.Tier;
        public string  Description => tempData.Description;
        public DanVisualQuality VisualQuality => visualQuality;
    }
}
