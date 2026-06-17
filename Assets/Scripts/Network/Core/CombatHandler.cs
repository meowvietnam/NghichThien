
using Base;
using LongDH.LongDHModules;
using Network.Extensions;
using Network.Models;
using System.Collections.Generic;
using UnityEngine;
using static INetworkEvent;

namespace Network.Core
{
    public class CombatHandler : IClientHandler
    {

        private IWinCombatBase winCombatCallBack;
        private ILoadCharCombat loadCharCallBack;

        private static MessageTag[] handler =
        {
                      MessageTag.WinCombatResult , MessageTag.LoadCharCombatResult
        };

        public MessageTag[] HandleTag => handler;
        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.WinCombatResult)
            {
                var result = reader.Read<WinCombatResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.MoneyVip += result.Reward.MoneyVip;
                    charData.Stats.LinhThach += result.Reward.Money;
                    charData.Progression.NgoDaoDiem = result.NewNgoDaoDiem;
                    var listItemRef = result.Reward.ItemRefs;
                    foreach (var item in listItemRef)
                    {
                        charData.InventoryManager.Add(item);

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
                        winCombatCallBack.SuccessWinCombatBase(result.Reward);
                    }
                    else
                    {
                        winCombatCallBack.FailWinCombatBase(result.Message);
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
         
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.WIN_COMBAT_BASE, WinCombat);
            Observer.AddObserver(EventList.Network.LOAD_CHAR_COMBAT, LoadCharCombat);

        }
        private void WinCombat(object[] data)
        {
            winCombatCallBack = data[0] as INetworkEvent.IWinCombatBase;
            var ETypeCombat = (ETypeCombat)data[1];
            var CurrentHpUser = (int)data[2];
            var ListThanThongUse = (List<int>)data[3];
            var LevelEnemy = (int)data[4];
            var winCombat = new WinCombat()
            {
                TypeCombat = ETypeCombat,
                CurrentHPUser = CurrentHpUser,
                ListIdThanThong = ListThanThongUse,
                CurrentLevelEnemy = LevelEnemy
            };
            Client.Instance.Send(MessageTag.WinCombat, winCombat);
        }
        private void LoadCharCombat(object[] data)
        {
            loadCharCallBack = data[0] as INetworkEvent.ILoadCharCombat;
            Client.Instance.Send(MessageTag.LoadCharCombat);
        }
        public void OnLeave()
        {
        }
    }
}