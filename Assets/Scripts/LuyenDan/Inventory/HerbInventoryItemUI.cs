using UnityEngine;

namespace LuyenDan.Inventory
{
    public class HerbInventoryItemUI : InventoryItemUI
    {
        public HerbData Data { get; private set; }
        protected override void Init(IInventoryItemData data)
        {
            base.Init(data);
            Data = data as HerbData;
        }

        public bool Is(HerbType type)
        {
            return Data.Is(type);
        }
    } 
}
