
using Base;
using LongDH.LongDHModules;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using System.Collections.Generic;
using static INetworkEvent;

namespace Network.Core
{
    public class QuestHandler : IClientHandler
    {

        private ICompleteMainQuest completeMainQuestCallBack;
        private ICompleteSideQuest completeSideQuestCallBack;

        private static MessageTag[] handler =
        {
                      MessageTag.CompleteMainQuestResult, MessageTag.CompleteSideQuestResult
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
                    charData.QuestManager.CurrentCountSideQuest = result.CurrentSideQuest;
                }
                NetworkManager.Instance.Invoke(() =>
                {

                    if (result.Success)
                    {
                        completeSideQuestCallBack.SuccessCompleteSideQuest(result.MoneyVipRandom);
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