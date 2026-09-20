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
            Observer.AddObserver(EventList.Combat.SEND_SYNC_ENDTURN_BOSS, SendSyncEndTurnBoss);
        }


        private void UnregisterEvents()
        {
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_DAME, SendSyncDame);
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_ENDTURN_BOSS, SendSyncEndTurnBoss);
        }
        private void SendSyncDame(object[] data)
        {
            var model = data[0] as SyncDame;
            client.MemSend(MessageTag.SyncDame, model);
        }
        private void SendSyncEndTurnBoss(object[] data)
        {
            var model = data[0] as SyncEndTurnBoss;
            client.MemSend(MessageTag.SyncEndTurnBoss, model);
        }
        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.SyncDame)
            {
                var model = reader.MemRead<SyncDame>();
                Observer.Notify(EventList.Combat.RECEIVE_SYNC_DAME, model);
            }
            else if (tag == MessageTag.SyncEndTurnBoss)
            {
                var model = reader.MemRead<SyncEndTurnBoss>();
                Observer.Notify(EventList.Combat.RECEIVE_SYNC_ENDTURN_BOSS, model);
            }
        }
        public void OnLeave()
        {
            UnregisterEvents();
        }

        public MessageState State => MessageState.BossState;
    }
}