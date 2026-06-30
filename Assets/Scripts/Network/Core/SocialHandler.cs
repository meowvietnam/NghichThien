
using Base;
using LongDH.LongDHModules;
using LuyenDan;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using static INetworkEvent;
namespace Network.Core
{
    public class SocialHandler : IClientHandler
    {
        private IThinhGiaoNPC thinhGiaoCallBack;
        private ITangQuaNPC tangQuaCallBack;
        private IGacha gachaCallBack;

        private static MessageTag[] handler =
        {
              MessageTag.ThinhGiaoKyNangResult , MessageTag.TangQuaNpcResult , MessageTag.GachaResult ,
        };
      
        public MessageTag[] HandleTag => handler;

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.THINH_GIAO_NPC, ThinhGiaoKyNang);
            Observer.AddObserver(EventList.Network.TANG_QUA_NPC, TangQuaNpc);
            Observer.AddObserver(EventList.Network.GACHA, Gacha);

        }
        private void Gacha(object[] data)
        {
            gachaCallBack = data[0] as INetworkEvent.IGacha;
            var typeGacha = (ETypeGacha)data[1];
            bool isGacha10 = (bool)data[2];
            var gacha = new GachaModel()
            {
                TypeGacha = typeGacha,
                IsGacha10 = isGacha10,

            };
            Client.Instance.Send(MessageTag.Gacha, gacha);
        }
        private void TangQuaNpc(object[] data)
        {
            tangQuaCallBack = data[0] as INetworkEvent.ITangQuaNPC;

            var idNpc = (int)data[1];
            var itemRef = (ItemRef)data[2];
            var tangQua = new TangQuaNpc()
            {
                IdChar = idNpc,
                ItemRef = itemRef,

            };
            Client.Instance.Send(MessageTag.TangQuaNpc, tangQua);
        }
        private void ThinhGiaoKyNang(object[] data)
        {
            thinhGiaoCallBack = data[0] as INetworkEvent.IThinhGiaoNPC;
         
            var idNpc = (int)data[1];
            var idSkill = (ushort)data[2];
            var typeSkill = (ItemType)data[3];
            var thinhGiao = new ThinhGiaoKyNang()
            {
                IdChar = (int)idNpc,
                IdSkill = (int)idSkill,
                TypeSkill = typeSkill,
               
            };
            Client.Instance.Send(MessageTag.ThinhGiaoKyNang, thinhGiao);
        }
        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.ThinhGiaoKyNangResult)
            {
                var result = reader.Read<ThinhGiaoKyNangResult>();
                if (result.Success)
                {
                    Client.Instance.CharacterData.Social.RelationshipSetValue((ushort)result.IdChar,result.NewValueRelationship);
                    Client.Instance.CharacterData.InventoryManager.Add(result.ItemRefSkill);
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        thinhGiaoCallBack.SuccessThinhGiaoNpc(result.ItemRefSkill);
                    }
                    else
                    {
                        thinhGiaoCallBack.FailThinhGiaoNpc(result.Message);
                    }

                });
            }
            else if (tag == MessageTag.TangQuaNpcResult)
            {
                var result = reader.Read<TangQuaNpcResult>();
                if (result.Success)
                {
                    Client.Instance.CharacterData.Social.RelationshipSetValue((ushort)result.IdChar, result.NewValueRelationship);
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        tangQuaCallBack.SuccessTangQuaNpc();
                    }
                    else
                    {
                        tangQuaCallBack.FailTangQuaNpc(result.Message);
                    }

                });
            }
            else if (tag == MessageTag.GachaResult)
            {
                var result = reader.Read<GachaResult>();
                if (result.Success)
                {
                    Client.Instance.CharacterData.MoneyVip = result.NewTienNgoc;
                    foreach (var item in result.Reward.ItemRefs)
                    {
                        Client.Instance.CharacterData.InventoryManager.Add(item);

                    }
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        gachaCallBack.SuccessGacha(result.Reward);
                    }
                    else
                    {
                        gachaCallBack.FailGacha(result.Message);
                    }

                });
            }
        }

        public void OnLeave()
        {
        }
    }
}

