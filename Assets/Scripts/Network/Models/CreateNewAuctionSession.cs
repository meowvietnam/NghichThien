using System;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    [Serializable]
    public partial class CreateNewAuctionSession
    {
        public int Price { get; set; }
        public ItemRef Item { get; set; }
    }
}