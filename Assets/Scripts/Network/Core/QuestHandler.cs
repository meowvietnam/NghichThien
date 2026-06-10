
using Base;
using LongDH.LongDHModules;
using Network.Extensions;
using Network.Models;
using static INetworkEvent;

namespace Network.Core
{
    public class QuestHandler : IClientHandler
    {

        private ICompleteMainQuest completeMainQuestCallBack;
        private static MessageTag[] handler =
        [
                      MessageTag.CompleteMainQuestResult,
        ];

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
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.COMPLETE_MAINQUEST, CompleteMainQuest);

        }
        private void CompleteMainQuest(object[] data)
        {
            completeMainQuestCallBack = data[0] as INetworkEvent.ICompleteMainQuest;
            var currentMainQuestClient = (int)data[1];
            Client.Instance.Send(MessageTag.CompleteMainQuest, currentMainQuestClient);
        }
        public void OnLeave()
        {
        }
    }
}