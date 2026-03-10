using System;
using Network;
using Network.Extensions;

namespace LuyenDan
{
    public class ItemRef : IMemorySerializable<ItemRef>
    {
        public int Id { get; private set; }
        public ItemType Type { get; private set; }
        public int Count { get; private set; }
        public Action<int> OnCountChanged { get; set; }
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