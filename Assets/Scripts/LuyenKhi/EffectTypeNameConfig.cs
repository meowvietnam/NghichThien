using System;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class EffectTypeNameConfig
    {
       [field: SerializeField] public string Name { get; set; } 
       [field: SerializeField] public EffectType Type { get; set; }
    }
}