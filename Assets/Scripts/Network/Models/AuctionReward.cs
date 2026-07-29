using System.Collections.Generic;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class AuctionReward
    {
        public string Content { get; set; }
        public ItemRef Item { get; set; }
        public List<FurnaceData> FurnaceDatas { get; set; }
    }
}