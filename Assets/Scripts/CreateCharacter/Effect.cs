using System;
using LuyenKhi;
using MemoryPack;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    [MemoryPackable]
    public partial class Effect
    {
        [field: SerializeField] public EffectType Type { get; set; }
        [field: SerializeField] public byte Value { get; set; }
    }
}