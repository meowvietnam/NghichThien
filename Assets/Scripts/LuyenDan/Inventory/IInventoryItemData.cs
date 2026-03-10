using System;
using UnityEngine;

namespace LuyenDan.Inventory
{
    public interface IInventoryItemData
    {
        int Id { get; }
        Sprite Icon { get; }
        string Name { get; }
        Tier Tier { get; }
        string Description { get; }
    }
}