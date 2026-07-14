using System.Collections.Generic;
using System.Linq;
using LuyenDan;
using MemoryPack;
using Network.Extensions;
using Network.Extensions.MemoryPack;

namespace Network
{
    [MemoryPackable]
    public partial class RewardItems
    {
        public List<FurnaceData> Furnaces { get; set; } = new List<FurnaceData>();
        public List<ItemRef> OtherItems { get; set; } = new List<ItemRef>();
    }
}