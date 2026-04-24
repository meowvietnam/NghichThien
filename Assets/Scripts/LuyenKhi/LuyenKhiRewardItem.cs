using System;
using System.Collections.Generic;
using System.Linq;
using LuyenKhi;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class LuyenKhiRewardItem
    {
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public EquipmentTier Tier { get; set; }
        [field: SerializeField] public SubEquipmentType Type { get; set; }
        [field: SerializeField, PreviewField(75)] public Sprite Icon { get; set; }
    }
}