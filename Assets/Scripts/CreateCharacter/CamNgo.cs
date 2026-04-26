using System;
using Base;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class CamNgo : IMemorySerializable<CamNgo>, IDeepClone<CamNgo>
    {
        [field: SerializeField]
        public ushort Id { get; set; }
        [field: SerializeField]
        public ushort Value { get; set; }
        public void Deserialize(IReader reader, ref CamNgo value)
        {
            value.Id = reader.Read<ushort>();
            value.Value = reader.Read<ushort>();
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Equals(CamNgo? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }
        public int ByteSize()
        {
            return Id.ByteSize() + Value.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Value);
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