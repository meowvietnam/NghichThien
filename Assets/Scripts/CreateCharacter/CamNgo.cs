using System;
using Base;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class CamNgo : IMemorySerializable<CamNgo>, IDeepClone<CamNgo>, IEquatable<CamNgo>
    {
        [field: SerializeField]
        public BaseSkill.ETypeSkill Id { get; set; }
        [field: SerializeField]
        public int Value { get; set; }
        public void Deserialize(IReader reader, ref CamNgo value)
        {
            value.Id = reader.Read<BaseSkill.ETypeSkill>();
            value.Value = reader.Read<int>();
        }
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