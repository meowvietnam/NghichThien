using System;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class EquipmentTierConfig
    {
        [field: SerializeField] public EquipmentTier Tier { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public int DayCost { get; private set; }
    }
}