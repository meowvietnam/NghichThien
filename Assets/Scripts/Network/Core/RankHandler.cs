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
            var callback = (IGetRank)data[0];
            var rankType = (RankType)data[1];
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
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.ResetArenaRank)
            {
                var charData = Client.Instance.CharacterData;
                var arena = charData.ArenaManager;
                arena.Points = 0;
                arena.CurrentArenaCount = 0;
                foreach (var arenaReward in arena.Rewards)
                {
                    arenaReward.IsClaim = false;
                }

                NetworkManager.Instance.Invoke(() => { Observer.Notify(EventList.Network.RESET_ARENA_RANK); });
            }
        }

        public void OnLeave()
        {
            Observer.RemoveObserver(EventList.Network.GET_RANK, GetRank);
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static MessageTag[] handler = { MessageTag.ResetArenaRank };
        public MessageTag[] HandleTag => handler;
    }
}