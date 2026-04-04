using Network.Callback;
using Network.Extensions;
using Network.Models;

namespace Network.ClientState
{
    public class LuyenDan : IClientState
    {
        private ILuyenDanCallback callback;
        public LuyenDan(ILuyenDanCallback callback)
        {
            this.callback = callback;
        }
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
          
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.LuyenDan;
    }
}