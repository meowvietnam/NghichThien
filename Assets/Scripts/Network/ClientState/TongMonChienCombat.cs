using System.Collections.Generic;
using AllMaps.EventReward;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Network.ClientState
{
    /// <summary>
    /// State khi đang đánh một trận Tông môn chiến: PvP thời gian thực (chuyển tiếp Sync* qua server)
    /// hoặc PvE với rối. Kết quả gửi bằng WarBattleReport; server xác thực và trả WarBattleEnded.
    /// HP ở đây là HP chiến: tuyệt đối không ghi vào CharacterData.Stats.Hp.
    /// </summary>
    public class TongMonChienCombat : IClientState
    {
        private readonly Client client;
        private readonly WarBattleMatched matched;
        private INetworkEvent.IResultCombat resultCombatCallback;
        private bool readySent;
        private bool reported;
        private WarBattleEnded ended;
        private bool shown;

        public TongMonChienCombat(WarBattleMatched matched)
        {
            client = Client.Instance;
            this.matched = matched;
        }

        public WarBattleMatched Matched => matched;

        /// <summary>Kết quả trận gần nhất (để scene chiến hiển thị thông báo sau khi quay lại).</summary>
        public static WarBattleEnded LastEnded { get; private set; }

        public void OnEnter()
        {
            Observer.AddObserver(EventList.Combat.SEND_SYNC_ADJUST_LINHKHI, SyncAdjustLinhKhi);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_BUFF, SyncBuff);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_DAME, SyncDame);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_ENDTURN, SyncEndTurn);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_HEALING, SyncHealing);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_PLAYSKILL, SyncPlaySkill);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_USE_PHAPBAO, SyncUsePhapBao);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_USE_EFFECTSKILL, SyncUseEffectSkill);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_CP_DYNAMIC, SyncCPDynamic);
            Observer.AddObserver(EventList.Combat.SEND_SYNC_REMOVE_EFFECTSKILLNODESTROY, SyncRemoveEffectSkillNoDestroy);
            Observer.AddObserver(EventList.Network.RESULT_COMBAT_REALTIME, ResultCombatRealtime);
            Observer.AddObserver(EventList.Network.RESULT_COMBAT_TONG_MON_CHIEN, ResultCombatPuppet);
        }

        public void SendReady()
        {
            if (readySent) return;
            readySent = true;
            client.Send(MessageTag.WarReadyCombat);
        }

        private void ResultCombatRealtime(object[] data)
        {
            SendReport(data, 0);
        }

        private void ResultCombatPuppet(object[] data)
        {
            SendReport(data, data.Length > 8 ? (int)data[8] : 0);
        }

        private void SendReport(object[] data, int enemyHp)
        {
            resultCombatCallback = data[0] as INetworkEvent.IResultCombat;
            if (reported)
            {
                TryShowEnded();
                return;
            }

            reported = true;
            var resultCombat = new ResultCombat
            {
                TypeCombat = (ETypeCombat)data[1],
                CurrentHPUser = (int)data[2],
                CurrentThanThuc = (int)data[3],
                CurrentThoNguyen = (int)data[4],
                ListIdThanThong = (List<int>)data[5],
                CurrentLevelEnemy = (int)data[6],
                TypeResultCombat = (ETypeResultCombat)data[7]
            };
            client.MemSend(MessageTag.WarBattleReport, new WarBattleReport
            {
                BattleId = matched.BattleId,
                Result = resultCombat,
                EnemyHp = enemyHp
            });
            TryShowEnded();
        }

        private void SyncAdjustLinhKhi(object[] data) => client.MemSend(MessageTag.SyncAdjustLinhKhi, data[0] as SyncAdjustLinhKhi);
        private void SyncBuff(object[] data) => client.MemSend(MessageTag.SyncBuff, data[0] as SyncBuff);
        private void SyncDame(object[] data) => client.MemSend(MessageTag.SyncDame, data[0] as SyncDame);
        private void SyncEndTurn(object[] data) => client.MemSend(MessageTag.SyncEndTurn, data[0] as SyncEndTurn);
        private void SyncHealing(object[] data) => client.MemSend(MessageTag.SyncHealing, data[0] as SyncHealing);
        private void SyncPlaySkill(object[] data) => client.MemSend(MessageTag.SyncPlaySkill, data[0] as SyncPlaySkill);
        private void SyncUsePhapBao(object[] data) => client.MemSend(MessageTag.SyncUsePhapBao, data[0] as SyncUsePhapBao);
        private void SyncUseEffectSkill(object[] data) => client.MemSend(MessageTag.SyncUseEffectSkill, data[0] as SyncEffectSkill);
        private void SyncCPDynamic(object[] data) => client.MemSend(MessageTag.SyncCPDynamic, data[0] as SyncCPDynamic);

        private void SyncRemoveEffectSkillNoDestroy(object[] data) =>
            client.MemSend(MessageTag.SyncRemoveEffectSkillNoDestroy, data[0] as SyncEffectSkill);

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            switch (tag)
            {
                case MessageTag.SyncStartTurn:
                {
                    var charId = reader.MemRead<int>();
                    NetworkManager.Instance.Invoke(() => Observer.Notify(EventList.Combat.SYNC_START_TURN, charId));
                    break;
                }
                case MessageTag.SyncAdjustLinhKhi:
                {
                    var model = reader.MemRead<SyncAdjustLinhKhi>();
                    NetworkManager.Instance.Invoke(() => Observer.Notify(EventList.Combat.RECEIVE_SYNC_ADJUST_LINHKHI, model));
                    break;
                }
                case MessageTag.SyncBuff:
                {
                    var model = reader.MemRead<SyncBuff>();
                    NetworkManager.Instance.Invoke(() => Observer.Notify(EventList.Combat.RECEIVE_SYNC_BUFF, model));
                    break;
                }
                case MessageTag.SyncDame:
                {
                    var model = reader.MemRead<SyncDame>();
                    NetworkManager.Instance.Invoke(() => Observer.Notify(EventList.Combat.RECEIVE_SYNC_DAME, model));
                    break;
                }
                case MessageTag.SyncEndTurn:
                {
                    var model = reader.MemRead<SyncEndTurn>();
                    NetworkManager.Instance.Invoke(() => Observer.Notify(EventList.Combat.RECEIVE_SYNC_ENDTURN, model));
                    break;
                }
                case MessageTag.SyncHealing:
                {
                    var model = reader.MemRead<SyncHealing>();
                    NetworkManager.Instance.Invoke(() => Observer.Notify(EventList.Combat.RECEIVE_SYNC_HEALING, model));
                    break;
                }
                case MessageTag.SyncPlaySkill:
                {
                    var model = reader.MemRead<SyncPlaySkill>();
                    NetworkManager.Instance.Invoke(() => Observer.Notify(EventList.Combat.RECEIVE_SYNC_PLAYSKILL, model));
                    break;
                }
                case MessageTag.SyncUsePhapBao:
                {
                    var model = reader.MemRead<SyncUsePhapBao>();
                    NetworkManager.Instance.Invoke(() => Observer.Notify(EventList.Combat.RECEIVE_SYNC_USE_PHAPBAO, model));
                    break;
                }
                case MessageTag.SyncUseEffectSkill:
                {
                    var model = reader.MemRead<SyncEffectSkill>();
                    NetworkManager.Instance.Invoke(() => Observer.Notify(EventList.Combat.RECEIVE_SYNC_USE_EFFECTSKILL, model));
                    break;
                }
                case MessageTag.SyncCPDynamic:
                {
                    var model = reader.MemRead<SyncCPDynamic>();
                    NetworkManager.Instance.Invoke(() => Observer.Notify(EventList.Combat.RECEIVE_SYNC_CP_DYNAMIC, model));
                    break;
                }
                case MessageTag.SyncRemoveEffectSkillNoDestroy:
                {
                    var model = reader.MemRead<SyncEffectSkill>();
                    NetworkManager.Instance.Invoke(() =>
                        Observer.Notify(EventList.Combat.RECEIVE_SYNC_REMOVE_EFFECTSKILLNODESTROY, model));
                    break;
                }
                case MessageTag.WarBattleEnded:
                {
                    var model = reader.MemRead<WarBattleEnded>();
                    NetworkManager.Instance.Invoke(() => OnBattleEnded(model));
                    break;
                }
            }
        }

        private void OnBattleEnded(WarBattleEnded model)
        {
            if (model.BattleId != matched.BattleId) return;
            ended = model;
            LastEnded = model;
            if (!reported)
            {
                // Server đã chốt trận trước khi client báo (đối thủ bỏ chạy/mất kết nối, hết giờ...):
                // kết thúc màn hình combat theo kết quả của server, không gửi báo cáo nữa.
                reported = true;
                var combatManager = Pool.Get<CombatManager>();
                if (combatManager != null)
                {
                    Pool.Get<GameManager>().currentECombatResult = ToCombatResult(model.Result);
                    resultCombatCallback = combatManager;
                }
            }

            TryShowEnded();
        }

        private void TryShowEnded()
        {
            if (shown || ended == null || resultCombatCallback == null) return;
            shown = true;
            resultCombatCallback.SuccessResultCombatBase(new RewardSnapshot(), ToCombatResult(ended.Result));
            if (!string.IsNullOrEmpty(ended.Message)) NotifyGroup.Instance?.Show(ended.Message);
        }

        private static ETypeResultCombat ToCombatResult(WarSideResult result)
        {
            switch (result)
            {
                case WarSideResult.Win: return ETypeResultCombat.Win;
                case WarSideResult.LoseDied: return ETypeResultCombat.LoseByDied;
                case WarSideResult.LoseRun: return ETypeResultCombat.LoseByRun;
                default: return ETypeResultCombat.LoseByTime;
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
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_USE_EFFECTSKILL, SyncUseEffectSkill);
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_CP_DYNAMIC, SyncCPDynamic);
            Observer.RemoveObserver(EventList.Combat.SEND_SYNC_REMOVE_EFFECTSKILLNODESTROY, SyncRemoveEffectSkillNoDestroy);
            Observer.RemoveObserver(EventList.Network.RESULT_COMBAT_REALTIME, ResultCombatRealtime);
            Observer.RemoveObserver(EventList.Network.RESULT_COMBAT_TONG_MON_CHIEN, ResultCombatPuppet);
        }

        public MessageState State => MessageState.TongMonChienCombat;
    }
}
