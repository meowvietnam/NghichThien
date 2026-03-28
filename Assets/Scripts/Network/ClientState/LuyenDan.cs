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
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.GetInventoryResult)
            {
                var result = reader.Read<GetInventoryResult>();
                callback.GetInventoryResult(result);
            }
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.LuyenDan;
    }
}