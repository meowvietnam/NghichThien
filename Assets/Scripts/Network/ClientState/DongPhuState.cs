using DP;
using Network.Extensions.MemoryPack;
using Network.Models;

namespace Network.ClientState
{
    public class DongPhuState : IClientState
    {
        private DongPhuPanel panel;
        public DongPhuState(DongPhuPanel panel)
        {
            this.panel = panel;
        }
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader =  message.Reader;
            NetworkManager.Instance.Invoke(() =>
            {
                OnMessage(tag, reader);
            });
        }

        private void OnMessage(MessageTag tag, IReader reader)
        {
            if (tag == MessageTag.GetDongPhuInfoResult)
            {
                var result = reader.MemRead<GetDongPhuInfoResult>();
                panel.OnGetDongPhuInfoResult(result);
            }
        }
        public void OnLeave()
        {
        }

        public MessageState State => MessageState.DongPhuState;
    }
}