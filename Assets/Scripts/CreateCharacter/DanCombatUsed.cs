using System;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class DanCombatUsed : IMemorySerializable<DanCombatUsed>, IEquatable<DanCombatUsed>
    {
        [field: SerializeField] public ushort Id { get; set; }
        [field: SerializeField] public ushort Count { get; set; }

        public void Deserialize(IReader reader, ref DanCombatUsed value)
        {
            value.Id = reader.Read<ushort>();
            value.Count = reader.Read<ushort>();
        }

        public int ByteSize()
        {
            return Id.ByteSize() + Count.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Count);
        }
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