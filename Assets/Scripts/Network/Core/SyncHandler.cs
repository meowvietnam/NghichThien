using CreateCharacter;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;

namespace Network.Core
{
    public class SyncHandler : IClientHandler
    {
        public void OnEnter()
        {
            Client.Instance.AddReconnectedListener(OnReconnected);
        }

        private void OnReconnected()
        {
            Client.Instance.Send(MessageTag.SyncCharacterData);
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.SyncCharacterDataResult)
            {
                var result = reader.MemRead<SyncCharacterDataResult>();
                if (result.Success)
                {
                    var charData = result.Character.Character;
                    Client.Instance.CharacterData.SyncFrom(charData);
                }
            }
        }
        public void OnLeave()
        {
            Client.Instance.RemoveReconnectedListener(OnReconnected);
        }
        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handler = { MessageTag.SyncCharacterDataResult };
        public MessageTag[] HandleTag => handler;
    }
}