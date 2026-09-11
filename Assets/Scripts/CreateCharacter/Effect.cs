using System;
using LuyenKhi;
using MemoryPack;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    public interface IEffect
    {
        EffectType Type { get; }
        int GetValue(); // Trả về int value
    }

    [Serializable]
    [MemoryPackable]
    public partial class Effect : IEffect
    {
        [field: SerializeField] public EffectType Type { get; set; }
        [field: SerializeField] public byte Value { get; set; }
        public int GetValue() => Value;
    }
}