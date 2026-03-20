using UnityEngine;

namespace LuyenDan.Inventory
{
    public class FurnaceInventoryItemUI : InventoryItemUI
    {
        public TempFurnaceData Data { get; private set; }
        public Tier Tier => Data.Tier;
        protected override void Init(IItemData data)
        {
            base.Init(data);
            Data = data as TempFurnaceData;
        }

        protected override void OnSelected()
        {
            LuyenDanPanel.Instance.SelectDanLu(this);
        }

        public override IItemData GetData()
        {
            return Data;
        }
    }
}
