using System;
using LuyenDan.Inventory;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class TempFurnaceData : IInventoryItemData
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite BigIcon { get; private set; }
        [field: SerializeField] public int PricePerDurability { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
    }
}