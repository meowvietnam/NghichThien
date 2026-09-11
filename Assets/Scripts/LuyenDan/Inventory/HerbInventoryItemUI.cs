using UnityEngine;

namespace LuyenDan.Inventory
{
    public class HerbInventoryItemUI : InventoryItemUI
    {
        public HerbData Data { get; private set; }

        protected override void Init(IItemData data)
        {
            base.Init(data);
            Data = data as HerbData;
        }

        protected override void OnSelected()
        {
            var instance = LuyenDanPanel.Instance;
            instance.Alchermy.Select(this);
            instance.HerbInventory.Close();
        }

        protected override void ShowInfo()
        {
            ToolTipItems.Instance.ShowInfoItem(Data, ItemRef);
        }

        public bool Is(HerbType type)
        {
            return Data.Is(type);
        }

        public override IItemData GetData()
        {
            return Data;
        }
    }
}