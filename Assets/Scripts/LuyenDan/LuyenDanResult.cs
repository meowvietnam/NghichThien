using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;

namespace LuyenDan
{
    public class LuyenDanResult : IMemorySerializable<LuyenDanResult>
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public List<ItemRef> ItemRefs { get; private set; }
        public int ByteSize()
        {
            return Success.ByteSize() + Message.ByteSize() + ItemRefs.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Success);
            writer.Write(Message);
            writer.WriteArray(ItemRefs);
        }
        public void Deserialize(IReader reader, ref LuyenDanResult value)
        {
            value.Success = reader.Read<bool>();
            value.Message = reader.Read();
            value.ItemRefs = reader.ReadArray<ItemRef>().ToList();
        }
    }
}