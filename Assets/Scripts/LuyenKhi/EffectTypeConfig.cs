using System;
using MemoryPack;
using Network;
using Network.Extensions;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    [MemoryPackable]
    public partial class EffectTypeConfig
    {
        [field: SerializeField] public EffectType Type { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public int Value { get; set; }
    }
}