using System;
using MemoryPack;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [MemoryPackable]
    [Serializable]
    public partial class MinMaxValueThreshold
    {
        [field: SerializeField] public int LevelThreshold { get; set; }
        [field: SerializeField] public int Min { get; set; }
        [field: SerializeField] public int Max { get; set; }

        public (int Min, int Max) GetMinMax()
        {
            return (Min, Max);
        }
    }

    [Serializable]
    [MemoryPackable]
    public partial class EffectTypeConfig
    {
        [field: SerializeField] public EffectType Type { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public int Value { get; set; }
        [field: SerializeField] public bool OverrideRange { get; set; }
        [field: SerializeField,ShowIf(nameof(OverrideRange))] public MinMaxValueThreshold[] Thresholds { get; set; }
    }
}