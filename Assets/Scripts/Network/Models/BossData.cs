using System;
using MemoryPack;
using UnityEngine;

namespace Network.Models
{
    [MemoryPackable]
    [Serializable]
    public partial class BossData
    {
        [field: SerializeField] public int SkinId { get; private set; }
        [field: SerializeField] public CharacterSnapshot Character { get; private set; }

    }
}
