using System;
using Base;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public record ThanThong : IMemorySerializable<ThanThong>, IDeepClone<ThanThong>
    {
        [field: SerializeField]
        public ushort Id { get; set; }
        public void Deserialize(IReader reader, ref ThanThong value)
        {
            value.Id = reader.Read<ushort>();
        }
        public int ByteSize()
        {
            return Id.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
        }

        public ThanThong DeepClone()
        {
            return new ThanThong()
            {
                Id = Id
            };
        }
    }
}