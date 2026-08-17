using System;
using LuyenDan;
using MemoryPack;
using System;

namespace Network.Models
{
    [Serializable]
    [MemoryPackable]
    public partial class CreateNewAuctionSession
    {
        public int Price;
        public ItemRef Item;
    }
}