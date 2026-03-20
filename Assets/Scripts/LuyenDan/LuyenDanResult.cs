using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;

namespace LuyenDan
{
    public class LuyenDanResult : IMemorySerializable<LuyenDanResult>
    {
        public LuyenDanResult(bool success, string message, List<ItemRef> itemRefs, int newDurability,  int newThoNguyen)
        {
            Success = success;
            Message = message;
            ItemRefs = itemRefs;
            NewDurability = newDurability;
            NewThoNguyen = newThoNguyen;
        }

        public LuyenDanResult()
        {
        }

        public bool Success { get; private set; }
        public string Message { get; private set; }
        public List<ItemRef> ItemRefs { get; private set; }
        public int NewDurability { get; private set; }
        public int NewThoNguyen { get; private set; }
        public int ByteSize()
        {
            return Success.ByteSize() + Message.ByteSize() + ItemRefs.ByteSize() + NewDurability.ByteSize() + NewThoNguyen.ByteSize();
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
            value.NewThoNguyen = reader.Read<int>();
        }
    }
}