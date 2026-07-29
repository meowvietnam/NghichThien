using System;
using JetBrains.Annotations;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class CurrentAuctionItem
    {
        public ItemRef Item { get; set; }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan RemainingTime { get; set; }
        public string PlayerBid { get; set; }
        public int CurrentPrice { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
    }
}