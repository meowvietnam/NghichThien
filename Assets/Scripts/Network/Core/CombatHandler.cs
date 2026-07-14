using Base;
using LongDH.LongDHModules;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using System.Collections.Generic;
using System.Linq;
using Network.Callback;
using UnityEngine;
using static INetworkEvent;

namespace Network.Core
{
    public class CombatHandler : IClientHandler
    {
        private IResultCombat resultCombatCallBack;
        private ILoadCharCombat loadCharCallBack;
        private ILoadCharCombatArena loadCharArenaCallBack;

        private static MessageTag[] handler =
        {
            MessageTag.ResultCombatResult,
            MessageTag.LoadCharCombatResult,
            MessageTag.LoadCharCombatArenaResult,
        };

        public MessageTag[] HandleTag => handler;

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.ResultCombatResult)
            {
                var result = reader.Read<ResultCombatResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.MoneyVip += result.Reward.MoneyVip;
                    charData.Stats.LinhThach += result.Reward.Money;
                    charData.Progression.NgoDaoDiem = result.NewNgoDaoDiem;
                    charData.ArenaManager.Points = result.NewPointRank;
                    charData.Stats.TamCanh = result.NewTamCanh;
                    if (result.Reward?.ItemRefs != null)
                    {
                        foreach (var item in result.Reward.ItemRefs)
                        {
                            charData.InventoryManager.Add(item);
                        }
                    }

                    charData.Stats.Hp = result.NewHP;
                    foreach (var item in result.NewCamNgo)
                    {
                        charData.InventoryManager.AddOrUpdate(item);
                    }

                    charData.EquipmentManager.DanCombatUseds.Clear();
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        resultCombatCallBack.SuccessWinCombatBase(result.Reward, result.TypeResultCombat);
                    }
                    else
                    {
                        resultCombatCallBack.FailWinCombatBase(result.Message);
                    }
                });
            }
            else if (tag == MessageTag.LoadCharCombatResult)
            {
                var result = reader.Read<LoadCharCombatResult>();

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        loadCharCallBack.SuccessLoadCharCombat(result.Client);
                    }
                    else
                    {
                        loadCharCallBack.FailLoadCharCombat(result.Message);
                    }
                });
            }
            else if (tag == MessageTag.LoadCharCombatArenaResult)
            {
                var result = reader.Read<LoadCharCombatResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.MoneyVip = result.NewMoneyVip;
                    charData.ArenaManager.CurrentArenaCount = result.CurrentArenaCount;
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        loadCharArenaCallBack.SuccessLoadCharCombatArena(result.Client, result.Enemy);
                    }
                    else
                    {
                        loadCharArenaCallBack.FailLoadCharCombatArena(result.Message);
                    }
                });
            }
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.RESULT_COMBAT, ResultCombat);
            Observer.AddObserver(EventList.Network.LOAD_CHAR_COMBAT, LoadCharCombat);
            Observer.AddObserver(EventList.Network.LOAD_CHAR_COMBAT_ARENA, LoadCharCombatArena);
            Observer.AddObserver(EventList.Network.LOAD_RANK, LoadRank);
            Observer.AddObserver(EventList.Network.CLAIM_RANK_REWARD, ClaimRankReward);
        }

        private async void ClaimRankReward(object[] data)
        {
            var callback = data[0] as IClaimRankReward;
            var id = (long)data[1];
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.ClaimRankRewardRequest, id);
            var result = reader.MemRead<ClaimRankRewardResult>();
            if (result.Success)
            {
                var charData = client.CharacterData;
                charData.MoneyVip += result.MoneyVip;
                var reward = charData.ArenaManager.Rewards
                    .First(x => x.Id == result.Id);
                reward.IsClaim = true;
            }
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    callback?.RewardSuccess(result);
                }
                else
                {
                    callback?.FailClaimReward(result.Message);
                }
            });
        }

        private async void LoadRank(object[] data)
        {
            var callback = data[0] as ILoadRank;
            var reader = await Client.Instance.RequestWait(MessageTag.LoadRankRequest);
            var result = reader.MemRead<LoadRankResult>();
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    callback?.LoadRankSuccess(result);
                }
                else
                {
                    callback?.LoadRankFailure(result.Message);
                }
            });
        }

        private void ResultCombat(object[] data)
        {
            resultCombatCallBack = data[0] as INetworkEvent.IResultCombat;
            var ETypeCombat = (ETypeCombat)data[1];
            var CurrentHpUser = (int)data[2];
            var CurrentThanThuc = (int)data[3];
            var CurrentThoNguyen = (int)data[4];

            var ListThanThongUse = (List<int>)data[5];
            var LevelEnemy = (int)data[6];
            var TypeResultCombat = (ETypeResultCombat)data[7];
            var ResultCombat = new ResultCombat()
            {
                TypeCombat = ETypeCombat,
                CurrentHPUser = CurrentHpUser,
                ListIdThanThong = ListThanThongUse,
                CurrentLevelEnemy = LevelEnemy,
                TypeResultCombat = TypeResultCombat,
                CurrentThanThuc = CurrentThanThuc,
                CurrentThoNguyen = CurrentThoNguyen
            };
            Client.Instance.Send(MessageTag.ResultCombat, ResultCombat);
        }

        private void LoadCharCombat(object[] data)
        {
            loadCharCallBack = data[0] as ILoadCharCombat;
            Client.Instance.Send(MessageTag.LoadCharCombat);
        }

        private void LoadCharCombatArena(object[] data)
        {
            loadCharArenaCallBack = data[0] as ILoadCharCombatArena;
            Client.Instance.Send(MessageTag.LoadCharCombatArena);
        }

        public void OnLeave()
        {
            Observer.RemoveObserver(EventList.Network.RESULT_COMBAT, ResultCombat);
            Observer.RemoveObserver(EventList.Network.LOAD_CHAR_COMBAT, LoadCharCombat);
            Observer.RemoveObserver(EventList.Network.LOAD_CHAR_COMBAT_ARENA, LoadCharCombatArena);
            Observer.RemoveObserver(EventList.Network.LOAD_RANK, LoadRank);
            Observer.RemoveObserver(EventList.Network.CLAIM_RANK_REWARD, ClaimRankReward);
        }
    }
}
