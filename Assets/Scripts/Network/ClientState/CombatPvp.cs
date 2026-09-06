using System.Collections.Generic;
using LongDH.LongDHModules;
using LongDHModules;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Network.ClientState
{
    public class CombatPvp : IClientState
    {
        private Client client;
        private INetworkEvent.IResultCombat resultCombatCallback;

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
            Observer.AddObserver(EventList.Network.RESULT_COMBAT_REALTIME, ResultCombat);
            client.Send(MessageTag.ReadyCombatPvp);
        }

        private void ResultCombat(object[] data)
        {
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
            client.MemSend(MessageTag.ResultCombatPvp, resultCombat);
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
            Debug.Log($"Is data null: {model.Datas == null}");
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
            NetworkManager.Instance.Invoke(() => { OnMessage(tag, reader); });
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
            else if (tag == MessageTag.ResultCombatPvp)
            {
                var result = reader.MemRead<ResultCombatResult>();

                if (result.Success)
                {
                    var charData = client.CharacterData;
                    if (result.Reward != null)
                    {
                        charData.Claim(result.Reward);
                    }
                    charData.Progression.NgoDaoDiem = result.NewNgoDaoDiem;
                    charData.ArenaManager.Points = result.NewPointRank;
                    charData.Stats.TamCanh = result.NewTamCanh;
                    charData.Stats.Hp = result.NewHP;
                    foreach (var item in result.NewCamNgo)
                    {
                        charData.InventoryManager.AddOrUpdate(item);
                    }
                    charData.EquipmentManager.DanCombatUseds.Clear();
                    resultCombatCallback.SuccessResultCombatBase(result.Reward, result.TypeResultCombat);
                }
                else
                {
                    resultCombatCallback.FailResultCombatBase(result.Message);
                }
            }
            else if (tag == MessageTag.YouWinPvp)
            {
                var combatManager = Pool.Get<CombatManager>();
                combatManager.EndCombat(ETypeResultCombat.Win);
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
            Observer.RemoveObserver(EventList.Network.RESULT_COMBAT_REALTIME, ResultCombat);
        }

        public MessageState State => MessageState.CombatPvp;
    }
}