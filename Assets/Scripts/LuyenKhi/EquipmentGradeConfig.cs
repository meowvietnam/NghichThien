using System;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class EquipmentGradeConfig
    {
        [field: SerializeField] public int MinGrade { get; private set; }
        [field: SerializeField] public EquipmentTier Tier { get; private set; }
        [field: SerializeField] public EquipmentGrade Grade { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
    }
}