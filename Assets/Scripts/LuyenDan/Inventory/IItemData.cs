using System;
using Base;
using UnityEngine;

namespace LuyenDan.Inventory
{
    public interface IItemData
    {
        int Id { get; }
        Sprite Icon { get; }
        string Name { get; }
        Tier Tier { get; }
        string Description { get; }
    }
}