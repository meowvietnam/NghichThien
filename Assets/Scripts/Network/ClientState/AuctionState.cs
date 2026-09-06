using Network.Callback;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Network.ClientState
{
    public class AuctionState : IClientState
    {
        private IAuctionCallback callback;
        private Client client;
        private NetworkManager net;
        public AuctionState(IAuctionCallback callback)
        {
            this.callback = callback;
            client = Client.Instance;
            net = NetworkManager.Instance;
        }
        public void OnEnter()
        {
            client.AddReconnectedListener(OnReconnected);
        }

        private void OnReconnected()
        {
            JoinAuction();
        }

        public void AuctionBid(int value)
        {
            client.Send(MessageTag.AuctionBid, value);
        }
        public void JoinAuction()
        {
            client.Send(MessageTag.JoinAuction);
        }
        public void GetAuctionData()
        {
            client.Send(MessageTag.GetAuctionData);
        }
        public void GetInfo()
        {
            client.Send(MessageTag.GetAuctionInfo);
        }
        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.JoinAuctionResult)
            {
                var result = reader.MemRead<JoinAuctionResult>();
                if (result.Success)
                {
                    var moneyVip = result.AuctionData.MyMoneyVip;
                    client.CharacterData.MoneyVip = moneyVip;
                }
                net.Invoke(() =>
                {
                    callback.OnJoinAuctionResult(result);
                });
            }
            else if (tag == MessageTag.GetAuctionDataResult)
            {
                var result = reader.MemRead<GetAuctionDataResult>();
                if (result.Success)
                {
                    var moneyVip = result.AuctionData.MyMoneyVip;
                    client.CharacterData.MoneyVip = moneyVip;
                }
                net.Invoke(() =>
                {
                    callback.OnGetAuctionDataResult(result);
                });
            }
            else if (tag == MessageTag.AuctionBidResult)
            {
                var result = reader.MemRead<AuctionBidResult>();
                if (result.Success)
                {
                    client.CharacterData.MoneyVip = result.NewMoneyVip;
                }
                net.Invoke(() =>
                {
                    callback.OnAuctionBidResult(result);
                });
            }
            else if (tag == MessageTag.ReplaceAuctionPlayer)
            {
                var replaceAuctionPlayer = reader.MemRead<ReplaceAuctionPlayer>();
                net.Invoke(() =>
                {
                    callback.ReplacePlayer(replaceAuctionPlayer);
                });
            }
            else if (tag == MessageTag.GetAuctionInfoResult)
            {
                var result = reader.MemRead<GetAuctionInfoResult>();
                net.Invoke(() =>
                {
                    callback.OnGetAuctionInfoResult(result);
                });
            }
            else if (tag == MessageTag.HighestBidChanged)
            {
                var highestBid = reader.MemRead<HighestBidChanged>();
                net.Invoke(() =>
                {
                    callback.HighestBidChanged(highestBid);
                });
            }
            else if (tag == MessageTag.HostChat)
            {
                var msg = reader.MemRead<string>();
                net.Invoke(() =>
                {
                    callback.HostChat(msg);
                });
            }
            else if (tag == MessageTag.NewAuctionSession)
            {
                var currentItem = reader.MemRead<Models.CurrentAuctionItem>();
                net.Invoke(() =>
                {
                    callback.NewAuctionSession(currentItem);
                });
            }
        }
        public void OnLeave()
        {
            client.RemoveReconnectedListener(OnReconnected);
        }
        public void OnRefund(RefundAuctionBid refundBid)
        {
            net.Invoke(() =>
            {
                callback.RefundAuctionBid(refundBid);
            });
        }
        public MessageState State => MessageState.Auction;

        public void OnReward(AuctionReward reward)
        {
            net.Invoke(() =>
            {
                callback.AuctionReward(reward);
            });
        }
    }
}