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
        [field: SerializeField] public byte SlotIndex { get; set; }
        public void Deserialize(IReader reader, ref ThanThongEquip value)
        {
            value.Id = reader.Read<ushort>();
            value.SlotIndex = reader.Read<byte>();
        }
        public int ByteSize()
        {
            return Id.ByteSize() + SlotIndex.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(SlotIndex);
        }
    }
}