using System;
using MemoryPack;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    [MemoryPackable]
    public partial record ThanThongEquip
    {
        [field: SerializeField] public ushort Id { get; set; }
        [field: SerializeField] public byte SlotIndex { get; set; }
    }
}