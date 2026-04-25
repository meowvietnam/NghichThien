using System;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class EffectTypeConfig
    {
        [field: SerializeField] public EffectType Type { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public int Value { get; set; }
    }
}