namespace Network.ClientState
{
    public class TrungSinh : IClientState
    {
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.TrungSinh;
    }
}