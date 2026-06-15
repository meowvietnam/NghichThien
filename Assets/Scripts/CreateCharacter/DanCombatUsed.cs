using System;
using MemoryPack;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    [MemoryPackable]
    public partial class DanCombatUsed : IEquatable<DanCombatUsed>
    {
        [field: SerializeField] public ushort Id { get; set; }
        [field: SerializeField] public ushort Count { get; set; }
        public bool Equals(DanCombatUsed other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}