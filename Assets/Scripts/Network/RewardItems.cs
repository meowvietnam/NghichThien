using System.Collections.Generic;
using System.Linq;
using LuyenDan;
using Network.Extensions;
using Network.Extensions.MemoryPack;

namespace Network
{
    public class RewardItems : IMemorySerializable<RewardItems>
    {
        public List<FurnaceData> Furnaces { get; set; } = new List<FurnaceData>();
        public List<ItemRef> OtherItems { get; set; } = new List<ItemRef>();
        public void Deserialize(IReader reader, ref RewardItems value)
        {
            value.Furnaces = reader.ReadArray<FurnaceData>().ToList();
            value.OtherItems = reader.ReadArray<ItemRef>().ToList();
        }

        public int ByteSize()
        {
            return Furnaces.ByteSize() + OtherItems.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(Furnaces);
            writer.WriteArray(OtherItems);
        }
    }
}