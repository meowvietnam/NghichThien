using System;
using Base;
using MemoryPack;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    [MemoryPackable]
    public partial record NgoDao : IDeepClone<NgoDao>
    {
        [field: SerializeField] public ushort Id { get; set; }

        public NgoDao DeepClone()
        {
            return new NgoDao { Id = Id };
        }
    }
}