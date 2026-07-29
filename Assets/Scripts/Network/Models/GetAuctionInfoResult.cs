using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class GetAuctionInfoResult : Result
    {
        public AuctionInfo AuctionInfo { get; set; }
    }
}