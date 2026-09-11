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
    public partial class CamNgo : IDeepClone<CamNgo>, IEquatable<CamNgo>
    {
        [field: SerializeField] public BaseSkill.ETypeSkill Id { get; set; }
        [field: SerializeField] public int Value { get; set; }

        public override int GetHashCode()
        {
            return (int)Id;
        }

        public bool Equals(CamNgo? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public CamNgo DeepClone()
        {
            return new CamNgo()
            {
                Id = Id,
                Value = Value
            };
        }
    }
}