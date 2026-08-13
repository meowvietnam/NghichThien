using Base;
using LongDH.LongDHModules;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using System.Collections.Generic;
using System.Linq;
using AllMaps;
using Firebase.Analytics;
using static INetworkEvent;

namespace Network.Core
{
    public class QuestHandler : IClientHandler
    {
        private ICompleteMainQuest completeMainQuestCallback;
        private ICompleteSideQuest completeSideQuestCallback;
        private IReLoadTask reloadTaskCallback;
        private IWatchAdsSideQuest watchAdsSideQuestCallback;

        private static MessageTag[] handler =
        {
            MessageTag.CompleteMainQuestResult,
            MessageTag.CompleteSideQuestResult,
            MessageTag.ReloadDailyQuestResult,
            MessageTag.WatchAdsSideQuestResult,
        };

        public MessageTag[] HandleTag => handler;

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.CompleteMainQuestResult)
            {
                var result = reader.Read<CompleteMainQuestResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.QuestManager.CurrentMainQuest = result.CurrentMainQuest;
                    charData.Stats.LinhThach = result.Money;
                    charData.MoneyVip = result.MoneyVip;
                    var rewardItems = result.RewardItems;
                    if (rewardItems != null)
                    {
                        charData.InventoryManager.AddToInventory(rewardItems);
                    }
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        completeMainQuestCallback.SuccessCompleteMainQuest();
                    }
                    else
                    {
                        completeMainQuestCallback.FailMainQuest(result.Message);
                    }
                });
            }
            else if (tag == MessageTag.CompleteSideQuestResult)
            {
                var result = reader.Read<CompleteSideQuestResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.MoneyVip += result.MoneyVipRandom;
                    charData.Stats.LinhThach += result.LinhThachRandom;
                    charData.QuestManager.CurrentCountSideQuest = result.CurrentSideQuest;
                    charData.Stats.TamCanh = result.NewTamCanh;
                    if (result.CurrentSideQuest == 10)
                    {
                        FirebaseAnalytics.LogEvent("complete_full_side_quest_day", "done", 1);
                    }
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        completeSideQuestCallback.SuccessCompleteSideQuest(result.MoneyVipRandom,
                            result.LinhThachRandom);
                    }
                    else
                    {
                        completeSideQuestCallback.FailCompleteSideQuest(result.Message);
                    }
                });
            }
            else if (tag == MessageTag.ReloadDailyQuestResult)
            {
                var client = Client.Instance;
                var result = reader.MemRead<ReloadDailyQuestResult>();
                if (result.Success)
                {
                    var listQuest = result.DailyQuests;
                    var eDailyQuest = result.EDailyQuest;
                    var questManager = client.CharacterData.QuestManager;
                    questManager.DailyQuests[eDailyQuest] = listQuest;
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        reloadTaskCallback?.SuccessReLoadTask(result.DailyQuests);
                    }
                    else
                    {
                        reloadTaskCallback?.FailReLoadTask(result.Message);
                    }
                });
            }
            else if (tag == MessageTag.WatchAdsSideQuestResult)
            {
                var result = reader.MemRead<WatchAdsSideQuestResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.MoneyVip = result.MoneyVip;
                    charData.Stats.LinhThach = result.Money;
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        watchAdsSideQuestCallback?.SuccessWatchAdsSideQuest(result);
                    }
                    else
                    {
                        watchAdsSideQuestCallback?.FailWatchAdsSideQuest(result.Message);
                    }
                });
            }
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.COMPLETE_MAINQUEST, CompleteMainQuest);
            Observer.AddObserver(EventList.Network.COMPLETE_SIDEQUEST, CompleteSideQuest);
            Observer.AddObserver(EventList.Network.LOAD_TASK, LoadTask);
            Observer.AddObserver(EventList.Network.DONE_TASK, DoneTask);
            Observer.AddObserver(EventList.Network.RELOAD_TASK, ReloadTask);
            Observer.AddObserver(EventList.Network.WATCH_ADS_SIDE_QUEST, WatchAdsSideQuest);
        }

        private void WatchAdsSideQuest(object[] data)
        {
            watchAdsSideQuestCallback = data[0] as IWatchAdsSideQuest;
            Client.Instance.Send(MessageTag.WatchAdsSideQuest);
        }

        private void ReloadTask(object[] data)
        {
            reloadTaskCallback = data[0] as IReLoadTask;
            var eDailyQuest = (EDailyQuest)data[1];
            var client = Client.Instance;
            client.Send(MessageTag.ReloadDailyQuest, eDailyQuest);
        }

        private async void DoneTask(object[] data)
        {
            var callback = data[0] as IDoneTask;
            var id = (long)data[1];
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.DoneDailyQuestRequest, id, 20000);
            var result = reader.MemRead<DoneQuestResult>();
            if (result.Success)
            {
                var quest = client.CharacterData.QuestManager
                    .DailyQuests.SelectMany(x => x.Value)
                    .First(x => x.Id == result.QuestId);
                quest.Claim();
            }

            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    callback?.SuccessDoneTask();
                }
                else
                {
                    callback?.FailDoneTask(result.Message);
                }
            });
        }

        private async void LoadTask(object[] data)
        {
            var callback = data[0] as ILoadTask;
            var eQuest = (EDailyQuest)data[1];
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.GetDailyQuestRequest, eQuest);
            var listQuest = reader.MemRead<List<DailyQuest>>();
            var questManager = client.CharacterData.QuestManager;
            questManager.DailyQuests[eQuest] = listQuest;
            NetworkManager.Instance.Invoke(() => { callback.SuccessLoadTask(listQuest); });
        }

        private void CompleteMainQuest(object[] data)
        {
            completeMainQuestCallback = data[0] as INetworkEvent.ICompleteMainQuest;
            var currentMainQuestClient = (int)data[1];
            Client.Instance.Send(MessageTag.CompleteMainQuest, currentMainQuestClient);
        }

        private void CompleteSideQuest(object[] data)
        {
            completeSideQuestCallback = data[0] as INetworkEvent.ICompleteSideQuest;
            var completeSideQuest = new CompleteSideQuest()
            {
                ListIdNpc = data[1] as List<int>
            };
            Client.Instance.Send(MessageTag.CompleteSideQuest, completeSideQuest);
        }

        public void OnLeave()
        {
            Observer.RemoveObserver(EventList.Network.COMPLETE_MAINQUEST, CompleteMainQuest);
            Observer.RemoveObserver(EventList.Network.COMPLETE_SIDEQUEST, CompleteSideQuest);
            Observer.RemoveObserver(EventList.Network.LOAD_TASK, LoadTask);
            Observer.RemoveObserver(EventList.Network.DONE_TASK, DoneTask);
            Observer.RemoveObserver(EventList.Network.RELOAD_TASK, ReloadTask);
            Observer.RemoveObserver(EventList.Network.WATCH_ADS_SIDE_QUEST, WatchAdsSideQuest);
        }
    }
}