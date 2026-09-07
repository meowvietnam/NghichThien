using System;
using Base;
using MemoryPack;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    [MemoryPackable]
    public partial record ThanThong : IDeepClone<ThanThong>
    {
        [field: SerializeField] public ushort Id { get; set; }

        public ThanThong DeepClone()
        {
            return new ThanThong()
            {
                Id = Id
            };
        }
    }
}