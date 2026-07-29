using System;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class GetAuctionDataResult : Result
    {
        public AuctionData AuctionData { get; set; }
    }
}