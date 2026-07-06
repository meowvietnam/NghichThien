namespace Network.Core
{
    public class AdsHandler : IClientHandler
    {
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.WatchAds)
            {
                
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static MessageTag[] handler = { MessageTag.WatchAds };
        public MessageTag[] HandleTag => handler;
    }
}