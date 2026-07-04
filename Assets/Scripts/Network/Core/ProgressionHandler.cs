using CreateCharacter;
using Network.Extensions;
using Network.Extensions.MemoryPack;

namespace Network.Core
{
    public class ProgressionHandler : IClientHandler
    {
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            var charData = Client.Instance.CharacterData;
            if (tag == MessageTag.LevelUpdated)
            {
                var levelUpdated = reader.MemRead<LevelUpdated>();
                levelUpdated.ApplyTo(charData);
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static MessageTag[] handler = { MessageTag.LevelUpdated };
        public MessageTag[] HandleTag { get; }
    }
}