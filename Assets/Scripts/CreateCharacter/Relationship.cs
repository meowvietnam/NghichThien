using System;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class Relationship : IMemorySerializable<Relationship>, IEquatable<Relationship>
    {
        [field: SerializeField]
        public ushort Npc { get; set; }
        [field: SerializeField]
        public ushort Points { get; set; }

        public void Deserialize(IReader reader, ref Relationship value)
        {
            value.Npc = reader.Read<ushort>();
            value.Points = reader.Read<ushort>();
        }
        public override int GetHashCode()
        {
            return Npc;
        }
        public int ByteSize()
        {
            return Npc.ByteSize() + Points.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Npc);
            writer.Write(Points);
        }
        public bool Equals(Relationship? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Npc == other.Npc;
        }
    }
}