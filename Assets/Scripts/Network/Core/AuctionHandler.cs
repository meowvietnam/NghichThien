using Network.ClientState;
using Network.Extensions.MemoryPack;
using Network.Models;
using Sirenix.Utilities;

namespace Network.Core
{
    public class AuctionHandler : IClientHandler
    {
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.RefundAuctionBid)
            {
                var client = Client.Instance;
                var refundBid = reader.MemRead<RefundAuctionBid>();
                client.CharacterData.MoneyVip = refundBid.NewMoneyVip;
                if (client.State is AuctionState state)
                {
                    state.OnRefund(refundBid);
                }
            }
            else if (tag == MessageTag.AuctionReward)
            {
                var client = Client.Instance;
                var reward = reader.MemRead<AuctionReward>();
                var furnaces = reward.FurnaceDatas;
                var inventory = client.CharacterData.InventoryManager;
                if (furnaces != null)
                {
                    inventory.Furnaces.AddRange(furnaces);
                }
                else
                {
                    inventory.Add(reward.Item);
                }

                if (client.State is AuctionState state)
                {
                    state.OnReward(reward);
                }
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handler = { MessageTag.RefundAuctionBid, MessageTag.AuctionReward };
        public MessageTag[] HandleTag => handler;
    }
}