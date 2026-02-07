using System;
using LuyenDan.Inventory;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class TempFurnaceData : IInventoryItemData
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public int Count { get; private set; }
        public Action<int> OnCountChanged { get; set; }
        [field: SerializeField] public int Durability { get;  private set; }
        public Action<int> OnDurabilityChanged { get; set; }
        [field: SerializeField] public string Description { get; private set; }
    }
}