namespace Network.ClientState
{
    public class TongMonChien : IClientState
    {
        private readonly Client client;

        public TongMonChien()
        {
            client = Client.Instance;
        }
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.TongMonChien;
    }
}