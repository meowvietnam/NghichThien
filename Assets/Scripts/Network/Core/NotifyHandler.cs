using CreateCharacter;
using Network.Extensions.MemoryPack;

namespace Network.Core
{
    public class NotifyHandler : IClientHandler
    {
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.Notify)
            {
                var msg = reader.MemRead<string>();
                NetworkManager.Instance.Invoke(() =>
                {
                    var notify = NotifyGroup.Instance;
                    if (notify != null)
                    {
                        notify.Show(msg);
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

        public static MessageTag[] handler = { MessageTag.Notify };
        public MessageTag[] HandleTag => handler;
    }
}