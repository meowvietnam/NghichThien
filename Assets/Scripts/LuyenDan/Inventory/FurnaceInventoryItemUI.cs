using Base;
using UnityEngine;

namespace LuyenDan.Inventory
{
    public class FurnaceInventoryItemUI : InventoryItemUI
    {
        public TempFurnaceData Data { get; private set; }
        public int FurnaceId => FData.SelfId;
        public Tier Tier => Data.Tier;
        public FurnaceData FData { get;  private set; }
        protected override void Init(IItemData data)
        {
            base.Init(data);
            Data = data as TempFurnaceData;
        }

        public void Init(FurnaceData data)
        {
            FData = data;
            Init(data.CreateRef());
        }
        protected override void OnSelected()
        {
            LuyenDanPanel.Instance.SelectDanLu(this);
        }

        protected override void ShowInfo()
        {
            LuyenDanPanel.Instance.ToolTipItems.ShowInfoItem(Data,ItemRef,FData);
        }

        public override IItemData GetData()
        {
            return Data;
        }
    }
}
