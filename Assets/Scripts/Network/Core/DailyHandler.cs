using LongDH.LongDHModules;
using Network.Extensions.MemoryPack;
using Network.Models;

namespace Network.Core
{
    public class DailyHandler : IClientHandler
    {
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.DailyReset)
            {
                var dailyReset = reader.MemRead<DailyReset>();
                Client.Instance.CharacterData.ApplyDailyReset(dailyReset);
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handler = { MessageTag.DailyReset };
        public MessageTag[] HandleTag => handler;
    }
}