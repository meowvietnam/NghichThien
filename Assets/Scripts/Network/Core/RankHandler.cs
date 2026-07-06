using AllMaps.ui_rank;
using LongDH.LongDHModules;
using Network.Callback;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Network.Core
{
    public class RankHandler : IClientHandler
    {
        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.GET_RANK, GetRank);
        }

        private async void GetRank(object[] data)
        {
            var callback = (IGetRank) data[0];
            var rankType = (RankType) data[1];
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.GetRankRequest, rankType);
            var result = reader.MemRead<GetRankResult>();
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    callback.GetRankSuccess(result.Rank);
                }
                else
                {
                    callback.GetRankFail(result.Message);
                }
            });
        }

        public void Handle(Message<MessageTag> message)
        {
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static MessageTag[] handler = { };
        public MessageTag[] HandleTag => handler;
    }
}