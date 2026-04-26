using System;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public record ThanThongEquip : IMemorySerializable<ThanThongEquip>
    {
        [field: SerializeField] public ushort Id { get; set; }
        public void Deserialize(IReader reader, ref ThanThongEquip value)
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
    }
}