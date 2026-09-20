using System;
using MemoryPack;
using Rank;
using UnityEngine;

namespace Network.Models
{
    [MemoryPackable]
    [Serializable]
    public partial class BossData
    {
        [field: SerializeField] public int SkinId { get; private set; }
        [field: SerializeField] public CharacterWrapper CharWrapper { get; private set; }

        public void Sync()
        {
            CharWrapper = new CharacterWrapper(CharWrapper.Character);
        }
    }
}