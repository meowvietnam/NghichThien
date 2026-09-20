using LongDH.LongDHModules;
using Network.Extensions.MemoryPack;
using Network.Models;

namespace Network.ClientState
{
    public class BossState : IClientState
    {
        private readonly Client client;
        public BossState()
        {
            client = Client.Instance;
        }
        public void OnEnter()
        {
            RegisterEvents();
        }
        private void RegisterEvents()
        {
            Observer.AddObserver(EventList.Combat.SEND_SYNC_DAME, SendSyncDame);
        }
        private void UnregisterEvents()
        {
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_DAME, SendSyncDame);
        }
        private void SendSyncDame(object[] data)
        {
            var model = data[0] as SyncDame;
            client.MemSend(MessageTag.SyncDame, model);
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.SyncDame)
            {
                
            }
        }
        public void OnLeave()
        {
            UnregisterEvents();
        }

        public MessageState State => MessageState.BossState;
    }
}