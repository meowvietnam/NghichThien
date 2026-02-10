using System;
using UnityEngine;

namespace LuyenDan.Inventory
{
    public interface IInventoryItemData
    {
        Sprite Icon { get; }
        string Name { get; }
        Tier Tier { get; }
        int Count { get; }
        Action<int> OnCountChanged { get; set; }
        int Durability { get; }
        Action<int> OnDurabilityChanged { get; set; }
        string Description { get; }
    }
}