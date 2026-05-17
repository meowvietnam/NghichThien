using LongDH.LongDHModules;
using Network.Extensions;
using Network.Models;

namespace Network.Core
{
    public class SkillHandler : IClientHandler
    {
        private static MessageTag[] handler = { MessageTag.LearnNgoDaoResult };
        public MessageTag[] HandleTag => handler;
        private INetworkEvent.ILearnNgoDao learnNgoDaoCallback;
        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.LEARN_NGODAO,LearnNgoDaoRequest);
        }

        private void LearnNgoDaoRequest(object[] data)
        {
            learnNgoDaoCallback = data[0] as INetworkEvent.ILearnNgoDao;
            var id = (int)data[1];
            Client.Instance.Send(MessageTag.LearnNgoDao, id);
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.LearnNgoDaoResult)
            {
                var result = reader.Read<LearnNgoDaoResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.InventoryManager.Add(result.NgoDao);
                    charData.ApplyBuff(result.Buff);
                    charData.Progression.NgoDaoDiem = result.NewNgoDaoDiem;
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        learnNgoDaoCallback.SuccessLearnNgoDao(result.Buff, result.NgoDao,result.NewNgoDaoDiem);
                    }
                    else
                    {
                        learnNgoDaoCallback.FailLearnNgoDao(result.Message);
                    }
                });
            }
        }
        public void OnLeave()
        {
        }
        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }
    }
}