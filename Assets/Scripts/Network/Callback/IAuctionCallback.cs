using Network.Models;

namespace Network.Callback
{
    public interface IAuctionCallback
    {
        void OnJoinAuctionResult(JoinAuctionResult result);
        void OnGetAuctionDataResult(GetAuctionDataResult result);
        void ReplacePlayer(ReplaceAuctionPlayer replacePlayer);
        void OnAuctionBidResult(AuctionBidResult result);
        void OnGetAuctionInfoResult(GetAuctionInfoResult result);
        void HighestBidChanged(HighestBidChanged highestBid);
        void RefundAuctionBid(RefundAuctionBid refundBid);
        void AuctionReward(AuctionReward reward);
        void HostChat(string msg);
        void NewAuctionSession(CurrentAuctionItem currentItem);
    }
}