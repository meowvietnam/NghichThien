using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDHModules;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;

namespace Network.ClientState
{
    public class BossState : IClientState
    {
        private readonly Client client;
        private INetworkEvent.IResultCombat resultCombatCallback;
        private bool resultPending;
        private bool resultReceived;
        public BossState()
        {
            client = Client.Instance;
        }
        public void OnEnter()
        {
            RegisterEvents();
            client.Send(MessageTag.ReadyFightingBoss);
        }
        private void RegisterEvents()
        {
            Observer.AddObserver(EventList.Combat.SEND_SYNC_DAME, SendSyncDame);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_ENDTURN_BOSS, SendSyncEndTurnBoss);
            Observer.AddObserver(EventList.Network.RESULT_COMBAT_BOSS, ResultCombatBoss);
        }

        private void UnregisterEvents()
        {
            Observer.RemoveObserver(EventList.Network.RESULT_COMBAT_BOSS, ResultCombatBoss);
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_DAME, SendSyncDame);
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_ENDTURN_BOSS, SendSyncEndTurnBoss);
        }
        private void ResultCombatBoss(object[] data)
        {
            if (resultPending || resultReceived) return;
            resultPending = true;
            resultCombatCallback = data[0] as INetworkEvent.IResultCombat;
            var eTypeCombat = (ETypeCombat)data[1];
            var currentHpUser = (int)data[2];
            var currentThanThuc = (int)data[3];
            var currentThoNguyen = (int)data[4];
            var listThanThongUse = (List<int>)data[5];
            var levelEnemy = (int)data[6];
            var typeResultCombat = (ETypeResultCombat)data[7];
            var resultCombat = new ResultCombat()
            {
                TypeCombat = eTypeCombat,
                CurrentHPUser = currentHpUser,
                ListIdThanThong = listThanThongUse,
                CurrentLevelEnemy = levelEnemy,
                TypeResultCombat = typeResultCombat,
                CurrentThanThuc = currentThanThuc,
                CurrentThoNguyen = currentThoNguyen
            };
            client.MemSend(MessageTag.ResultCombatBoss, resultCombat);
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
            NetworkManager.Instance.Invoke(() =>
            {
                OnMessage(tag,reader);
            });
        }

        private void OnMessage(MessageTag tag, IReader reader)
        {
            if (tag == MessageTag.SyncDame)
            {
                var model = reader.MemRead<SyncDame>();
                Observer.Notify(EventList.Combat.RECEIVE_SYNC_DAME, model);
            }
            else if (tag == MessageTag.FirstDailyBossReward)
            {
                var model = reader.MemRead<FirstDailyBossReward>();
                foreach (var r in model.Rewards)
                {
                    r.ClaimReward(client.CharacterData);
                }
                DOVirtual.DelayedCall(3, () =>
                {
                    var notify = Pool.Get<CombatManager>()?.notifyGroup ?? null;
                    notify?.Show($"Bạn nhận được {string.Join(", ", model.Rewards.Select(x => x.GetRichText()))}");
                });
            }
            else if (tag == MessageTag.SyncStartTurn)
            {
                var id = reader.MemRead<int>();
                Observer.Notify(EventList.Combat.SYNC_START_TURN, id);
            }
            else if (tag == MessageTag.SyncEndTurnBoss)
            {
                var model = reader.MemRead<SyncEndTurnBoss>();
                Observer.Notify(EventList.Combat.RECEIVE_SYNC_ENDTURN_BOSS, model);
            }
            else if (tag == MessageTag.BossHp)
            {
                Pool.Get<CombatManager>().char2.SetHp(reader.MemRead<int>());
            }
            else if (tag == MessageTag.BossEnded)
            {
                var killed = reader.MemRead<bool>();
                Pool.Get<CombatManager>().EndCombat(killed
                    ? ETypeResultCombat.Win : ETypeResultCombat.LoseByTime);
            }
            else if (tag == MessageTag.ResultCombatBoss)
            {
                var result = reader.MemRead<ResultCombatResult>();
                resultPending = false;
                resultReceived = result.Success;
                var callback = resultCombatCallback;
                resultCombatCallback = null;
                if (!result.Success)
                {
                    if (callback != null) callback.FailResultCombatBase(result.Message);
                    else NotifyGroup.Instance.Show(result.Message);
                    return;
                }
                var charData = client.CharacterData;
                charData.Stats.Hp = result.NewHP;
                charData.Stats.TamCanh = result.NewTamCanh;
                charData.Progression.NgoDaoDiem = result.NewNgoDaoDiem;
                charData.EquipmentManager.DanCombatUseds.Clear();
                callback?.SuccessResultCombatBase(result.Reward, result.TypeResultCombat);
            }
        }
        public void OnLeave()
        {
            UnregisterEvents();
        }

        public MessageState State => MessageState.BossState;
    }
}