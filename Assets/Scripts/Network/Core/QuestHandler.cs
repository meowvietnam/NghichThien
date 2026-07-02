using Base;
using LongDH.LongDHModules;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using System.Collections.Generic;
using System.Linq;
using AllMaps;
using static INetworkEvent;

namespace Network.Core
{
    public class QuestHandler : IClientHandler
    {
        private ICompleteMainQuest completeMainQuestCallBack;
        private ICompleteSideQuest completeSideQuestCallBack;

        private static MessageTag[] handler =
        {
            MessageTag.CompleteMainQuestResult,
            MessageTag.CompleteSideQuestResult,
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
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        completeMainQuestCallBack.SuccessCompleteMainQuest();
                    }
                    else
                    {
                        completeMainQuestCallBack.FailMainQuest(result.Message);
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
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        completeSideQuestCallBack.SuccessCompleteSideQuest(result.MoneyVipRandom,
                            result.LinhThachRandom);
                    }
                    else
                    {
                        completeSideQuestCallBack.FailCompleteSideQuest(result.Message);
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
        }

        private async void ReloadTask(object[] data)
        {
            var callback = data[0] as IReLoadTask;
            var eDailyQuest = (EDailyQuest)data[1];
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.ReloadDailyQuestRequest, eDailyQuest);
            var listQuest = reader.MemRead<List<DailyQuest>>();
            var questManager = client.CharacterData.QuestManager;
            questManager.DailyQuests[eDailyQuest] = listQuest;
            NetworkManager.Instance.Invoke(() =>
            {
                callback?.SuccessReLoadTask(listQuest);
            });
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
            completeMainQuestCallBack = data[0] as INetworkEvent.ICompleteMainQuest;
            var currentMainQuestClient = (int)data[1];
            Client.Instance.Send(MessageTag.CompleteMainQuest, currentMainQuestClient);
        }

        private void CompleteSideQuest(object[] data)
        {
            completeSideQuestCallBack = data[0] as INetworkEvent.ICompleteSideQuest;
            var completeSideQuest = new CompleteSideQuest()
            {
                ListIdNpc = data[1] as List<int>
            };
            Client.Instance.Send(MessageTag.CompleteSideQuest, completeSideQuest);
        }

        public void OnLeave()
        {
        }
    }
}