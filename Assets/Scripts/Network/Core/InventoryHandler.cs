using CreateCharacter;
using LongDH.LongDHModules;
using Network.Callback;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Core
{
    public class InventoryHandler : IClientHandler
    {
        private Action<GetInventoryResult> getInventoryCallback;
        private InventoryManager Inventory => Client.Instance.CharacterData.InventoryManager;
        private INetworkEvent.IEquipEquipment equipEquipmentCallBack;
        public InventoryHandler()
        {
        }

        public void OnEnter()
        {
            Client.Instance.Send(MessageTag.GetInventory);
            Observer.AddObserver(EventList.Network.EQUIP_EQUIPMENT, EquipEquipment);

        }
        private void EquipEquipment(object[] data)
        {
            equipEquipmentCallBack = data[0] as INetworkEvent.IEquipEquipment;
            var SelfId = (ushort)data[1];
            var TypeEquipment = (LuyenKhi.EquipmentType)data[2];
          
            var equipEquipment = new EquipEquipment()
            {
                Id = SelfId,
                TypeEquipment = TypeEquipment
            };
            Client.Instance.Send(MessageTag.EquipEquipment, equipEquipment);
        }
        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.GetInventoryResult)
            {
                var result = reader.Read<GetInventoryResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.SetInventory(result.InventoryManager);
                }
                else
                {
                    Debug.LogError(result.Message);
                }
            }
            else if (tag == MessageTag.EquipEquipmentResult)
            {
                var result = reader.Read<EquipEquipmentResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.EquipmentManager.EquipEquipment(result);
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        equipEquipmentCallBack.SuccessEquipEquipment();
                    }
                    else
                    {
                        equipEquipmentCallBack.FailEquipEquipment(result.Message);
                    }

                });
            }
        }

        public void OnLeave()
        {
            Observer.RemoveObserver(EventList.Network.EQUIP_EQUIPMENT, EquipEquipment);
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handlerTag = { MessageTag.GetInventoryResult , MessageTag.EquipEquipmentResult };
        public MessageTag[] HandleTag => handlerTag;
    }
}
