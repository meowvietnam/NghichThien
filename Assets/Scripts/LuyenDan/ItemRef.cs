using System;
using Network;
using Network.Extensions;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class ItemRef : IMemorySerializable<ItemRef>
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public ItemType Type { get; private set; }
        [field: SerializeField] public int Count { get; private set; }
        public Action<int> OnCountChanged { get; set; }
        public ItemRef() {}
        public ItemRef(int id, ItemType type, int count)
        {
            Id = id;
            Type = type;
            Count = count;
        }
        public int ByteSize()
        {
            return Id.ByteSize() + Type.ByteSize() + Count.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Type);
            writer.Write(Count);
        }
        public void Deserialize(IReader reader, ref ItemRef value)
        {
            value.Id = reader.Read<int>();
            value.Type = reader.Read<ItemType>();
            value.Count = reader.Read<int>();
        }
    }
}