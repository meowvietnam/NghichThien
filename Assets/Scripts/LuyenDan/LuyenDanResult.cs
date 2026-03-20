using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;

namespace LuyenDan
{
    public class LuyenDanResult : IMemorySerializable<LuyenDanResult>
    {
        public LuyenDanResult(bool success, string message, List<ItemRef> itemRefs, int newDurability)
        {
            Success = success;
            Message = message;
            ItemRefs = itemRefs;
            NewDurability = newDurability;
        }

        public LuyenDanResult()
        {
        }

        public bool Success { get; private set; }
        public string Message { get; private set; }
        public List<ItemRef> ItemRefs { get; private set; }
        public int NewDurability { get; set; }
        public int ByteSize()
        {
            return Success.ByteSize() + Message.ByteSize() + ItemRefs.ByteSize() + NewDurability.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Success);
            writer.Write(Message);
            writer.WriteArray(ItemRefs);
            writer.Write(NewDurability);
        }
        public void Deserialize(IReader reader, ref LuyenDanResult value)
        {
            value.Success = reader.Read<bool>();
            value.Message = reader.Read();
            value.ItemRefs = reader.ReadArray<ItemRef>().ToList();
            value.NewDurability = reader.Read<int>();
        }
    }
}