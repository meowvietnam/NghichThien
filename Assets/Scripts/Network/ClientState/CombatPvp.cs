using LongDH.LongDHModules;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Network.ClientState
{
    public class CombatPvp : IClientState
    {
        private Client client;
        public CombatPvp()
        {
            client = Client.Instance;
        }
        public void OnEnter()
        {
            Observer.AddObserver(EventList.Combat.SEND_SYNC_ADJUST_LINHKHI, SyncAdjustLinhKhi);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_BUFF, SyncBuff);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_DAME, SyncDame);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_ENDTURN, SyncEndTurn);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_HEALING, SyncHealing);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_PLAYSKILL, SyncPlaySkill);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_USE_PHAPBAO, SyncUsePhapBao);
            client.Send(MessageTag.ReadyCombatPvp);
        }



        private void SyncAdjustLinhKhi(object[] data)
        {
            var model = data[0] as SyncAdjustLinhKhi;
            client.MemSend(MessageTag.SyncAdjustLinhKhi, model);
        }
        private void SyncBuff(object[] data)
        {
            var model = data[0] as SyncBuff;
            client.MemSend(MessageTag.SyncBuff, model);
        }
        private void SyncDame(object[] data)
        {
            var model = data[0] as SyncDame;
            client.MemSend(MessageTag.SyncDame, model);
        }
        private void SyncEndTurn(object[] data)
        {
            var model = data[0] as SyncEndTurn;
            client.MemSend(MessageTag.SyncEndTurn, model);
        }
        private void SyncHealing(object[] data)
        {
            var model = data[0] as SyncHealing;
            client.MemSend(MessageTag.SyncHealing, model);
        }
        private void SyncPlaySkill(object[] data)
        {
            Debug.Log("Sync Play Skill");
            var model = data[0] as SyncPlaySkill;
            client.MemSend(MessageTag.SyncPlaySkill, model);
        }
        private void SyncUsePhapBao(object[] data)
        {
            var model = data[0] as SyncUsePhapBao;
            client.MemSend(MessageTag.SyncUsePhapBao, model);
        }
        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            NetworkManager.Instance.Invoke(() =>
            {
                OnMessage(tag,reader);
            });
        }

        private void OnMessage(MessageTag tag, IReader reader)
        {
            if (tag == MessageTag.SyncStartTurn)
            {
                var charId = reader.MemRead<int>();
                Observer.Notify(EventList.Combat.SYNC_START_TURN, charId);
            }
            else if (tag == MessageTag.SyncAdjustLinhKhi)
            {
                var model = reader.MemRead<SyncAdjustLinhKhi>();
                Observer.Notify(EventList.Combat.RECEIVE_SYNC_ADJUST_LINHKHI, model);
            }
            else if (tag == MessageTag.SyncBuff)
            {
                var model = reader.MemRead<SyncBuff>();
                Observer.Notify(EventList.Combat.RECEIVE_SYNC_BUFF, model);
            }
            else if (tag == MessageTag.SyncDame)
            {
                var model = reader.MemRead<SyncDame>();
                Observer.Notify(EventList.Combat.RECEIVE_SYNC_DAME, model);
            }
            else if (tag == MessageTag.SyncEndTurn)
            {
                var model = reader.MemRead<SyncEndTurn>();
                Observer.Notify(EventList.Combat.RECEIVE_SYNC_ENDTURN, model);
            }
            else if (tag == MessageTag.SyncHealing)
            {
                var model = reader.MemRead<SyncHealing>();
                Observer.Notify(EventList.Combat.RECEIVE_SYNC_HEALING, model);
            }
            else if (tag == MessageTag.SyncPlaySkill)
            {
                var model = reader.MemRead<SyncPlaySkill>();
                Observer.Notify(EventList.Combat.RECEIVE_SYNC_PLAYSKILL, model);
            }
            else if (tag == MessageTag.SyncUsePhapBao)
            {
                var model = reader.MemRead<SyncUsePhapBao>();
                Observer.Notify(EventList.Combat.RECEIVE_SYNC_USE_PHAPBAO, model);
            }
        }
        public void OnLeave()
        {
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_ADJUST_LINHKHI, SyncAdjustLinhKhi);
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_BUFF, SyncBuff);
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_DAME, SyncDame);
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_ENDTURN, SyncEndTurn);
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_HEALING, SyncHealing);
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_PLAYSKILL, SyncPlaySkill);
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_USE_PHAPBAO, SyncUsePhapBao);
        }

        public MessageState State => MessageState.CombatPvp;
    }
}