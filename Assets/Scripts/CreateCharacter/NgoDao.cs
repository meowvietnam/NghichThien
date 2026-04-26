using System;
using Base;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public record NgoDao : IMemorySerializable<NgoDao>, IDeepClone<NgoDao>
    {
        [field: SerializeField]
        public ushort Id { get; set; }

        public void Deserialize(IReader reader, ref NgoDao value)
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

        public NgoDao DeepClone()
        {
            return new NgoDao { Id = Id };
        }
    }
}