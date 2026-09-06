using System.Linq;
using System.Threading.Tasks;
using Firebase.Analytics;
using LongDH.LongDHModules;
using LongDHModules;
using Network.Callback;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Network.ClientState
{
    public class AllMaps : IClientState
    {
        private IAllMapsCallback callback;

        public AllMaps(IAllMapsCallback callback)
        {
            this.callback = callback;
        }

        public void OnEnter()
        {
            var client = Client.Instance;
            client.Send(MessageTag.GetPlayerOnlineThisNode);
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.MoveToMapResult)
            {
                var result = reader.Read<MoveToMapResult>();
                var charData = Client.Instance.CharacterData;

                if (result.Success)
                {
                    charData.Progression.IdCurrentNode = result.Path.Last();
                    charData.Progression.CurrentEnergy = result.Energy;
                    charData.Stats.TuoiTac = result.NewAge;
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (!result.Success && !charData.IsInfiniteEnergy)
                    {
                        Pool.Get<PopupConfirmAction>().ShowPopConfirmAction(
                            "Không đủ năng lượng\nXác nhận xem quảng cáo để hồi đầy năng lượng",
                            () =>
                            {
                                if (!charData.IsNoAds)
                                {
                                    AdsService.Instance.ShowRewardVideoAds("RewardEnergy",
                                        "Xem quảng cáo để hồi đầy năng lượng",
                                        () => { Client.Instance.Send(MessageTag.WatchAdsForEnergy); }, null, null);
                                }
                                else
                                {
                                    Client.Instance.Send(MessageTag.WatchAdsForEnergy);
                                }
                            });
                    }
                });
                callback.OnMoveToMapResult(result);
            }
            else if (tag == MessageTag.GetPlayerOnlineThisNodeResult)
            {
                var result = reader.Read<GetCharacterOnlineThisNodeResult>();
                callback.OnGetCharacterOnlineThisNode(result);
            }
            else if (tag == MessageTag.WatchAdsForEnergyResult)
            {
                var result = reader.Read<WatchAdsForEnergyResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.Progression.CurrentEnergy = result.NewEnergy;
                    NetworkManager.Instance.Invoke(() =>
                    {
                        Observer.Notify(EventList.GamePlay.MOVE_MAP_SUCCESS);
                        FirebaseAnalytics.LogEvent("ads_reward_complete");
                    });
                }
            }
            else if (tag == MessageTag.OtherPlayerMoving)
            {
                var playerMoving = reader.MemRead<PlayerMoving>();
                NetworkManager.Instance.Invoke(() =>
                {
                    callback.OnPlayerMoving(playerMoving);
                });
            }
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.AllMaps;
    }
}