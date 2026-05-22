using LongDH.LongDHModules;
using Network.Extensions;
using Network.Models;
using Spine;

namespace Network.Core
{
    public class SkillHandler : IClientHandler
    {
        private static MessageTag[] handler =
            { MessageTag.LearnNgoDaoResult, MessageTag.EquipThanThongResult, MessageTag.UnequipThanThongResult };

        public MessageTag[] HandleTag => handler;
        private INetworkEvent.ILearnNgoDao learnNgoDaoCallback;
        private INetworkEvent.IEquipThanThong equipThanThongCallback;
        private INetworkEvent.IUnEquipThanThong unequipThanThongCallback;

        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.LEARN_NGODAO, LearnNgoDaoRequest);
            Observer.AddObserver(EventList.Network.EQUIP_THANTHONG, EquipThanThong);
            Observer.AddObserver(EventList.Network.UNEQUIP_THANTHONG, UnEquipThanThong);
        }

        private void UnEquipThanThong(object[] data)
        {
            unequipThanThongCallback = data[0] as INetworkEvent.IUnEquipThanThong;
            var id = (int)data[1];
            Client.Instance.Send(MessageTag.UnequipThanThong, id);
        }

        private void EquipThanThong(object[] data)
        {
            equipThanThongCallback = data[0] as INetworkEvent.IEquipThanThong;
            var id = (ushort)data[1];
            var slotIndex = (byte)data[2];
            var equipThanThong = new EquipThanThong()
            {
                Id = id,
                Index = slotIndex,
            };
            Client.Instance.Send(MessageTag.EquipThanThong, equipThanThong);
        }

        private void LearnNgoDaoRequest(object[] data)
        {
            learnNgoDaoCallback = data[0] as INetworkEvent.ILearnNgoDao;
            var id = (int)data[1];
            Client.Instance.Send(MessageTag.LearnNgoDao, id);
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.LearnNgoDaoResult)
            {
                var result = reader.Read<LearnNgoDaoResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.InventoryManager.Add(result.NgoDao);
                    charData.ApplyBuff(result.Buff);
                    charData.Progression.NgoDaoDiem = result.NewNgoDaoDiem;
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        learnNgoDaoCallback.SuccessLearnNgoDao(result.Buff, result.NgoDao, result.NewNgoDaoDiem);
                    }
                    else
                    {
                        learnNgoDaoCallback.FailLearnNgoDao(result.Message);
                    }
                });
            }
            else if (tag == MessageTag.EquipThanThongResult)
            {
                var result = reader.Read<EquipThanThongResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.EquipThanThong(result);
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        equipThanThongCallback.SuccessEquipThanThong();
                    }
                    else
                    {
                        equipThanThongCallback.FailEquipThanThong(result.Message);
                    }
                });
            }
            else if (tag == MessageTag.UnequipThanThongResult)
            {
                var result = reader.Read<UnequipThanThongResult>();
                if (result.Success)
                {
                    var id = result.Id;
                    var charData = Client.Instance.CharacterData;
                    charData.EquipmentManager.UnEquipThanThong(id);
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        unequipThanThongCallback.SuccessUnEquipThanThong();
                    }
                    else
                    {
                        unequipThanThongCallback.FailUnEquipThanThong(result.Message);
                    }
                });
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }
    }
}