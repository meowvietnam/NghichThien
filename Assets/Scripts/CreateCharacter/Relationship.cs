using System;
using MemoryPack;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    [MemoryPackable]
    public partial class Relationship : IEquatable<Relationship>
    {
        [field: SerializeField] public ushort Npc { get; set; }
        [field: SerializeField] public int Points { get; set; }

        public override int GetHashCode()
        {
            return Npc;
        }
        public bool Equals(Relationship? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Npc == other.Npc;
        }
    }
}