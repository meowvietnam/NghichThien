using System;
using System.Collections.Generic;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using MemoryPack;
using Network.Extensions;
using Network.Models;
using Spine;

namespace Network.Core
{
    [MemoryPackable]
    public partial class Test
    {
        public string Message { get; set; }
        public Dictionary<int, string[]> Hellos { get; set; }
    }

    public class SkillHandler : IClientHandler
    {
        private static MessageTag[] handler =
        {
            MessageTag.LearnNgoDaoResult, MessageTag.EquipThanThongResult, MessageTag.UnequipThanThongResult,
            MessageTag.EquipCongPhapResult, MessageTag.UnequipCongPhapResult,
            MessageTag.TuLuyenResult, MessageTag.LinhNgoResult
        };

        public MessageTag[] HandleTag => handler;
        private INetworkEvent.ILearnNgoDao learnNgoDaoCallback;
        private INetworkEvent.IEquipThanThong equipThanThongCallback;
        private INetworkEvent.IUnEquipThanThong unequipThanThongCallback;
        private INetworkEvent.IEquipCongPhap equipCongPhapCallback;
        private INetworkEvent.IUnEquipCongPhap unequipCongPhapCallback;
        private INetworkEvent.ITuLuyen tuLuyenCallback;
        private INetworkEvent.ILinhNgo linhNgoCallback;

        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.LEARN_NGODAO, LearnNgoDaoRequest);
            Observer.AddObserver(EventList.Network.EQUIP_THANTHONG, EquipThanThong);
            Observer.AddObserver(EventList.Network.UNEQUIP_THANTHONG, UnEquipThanThong);
            Observer.AddObserver(EventList.Network.EQUIP_CONGPHAP, EquipCongPhap);
            Observer.AddObserver(EventList.Network.UNEQUIP_CONGPHAP, UnEquipCongPhap);
            Observer.AddObserver(EventList.Network.TU_LUYEN, TuLuyen);
            Observer.AddObserver(EventList.Network.LINH_NGO, LinhNgo);
        }

        private void LinhNgo(object[] data)
        {
            linhNgoCallback = data[0] as INetworkEvent.ILinhNgo;
            var id = (int)data[1];
            var type = (ItemType)data[2];
            var linhNgo = new LinhNgo()
            {
                Id = id,
                Type = type,
            };
            Client.Instance.Send(MessageTag.LinhNgo, linhNgo);
        }

        private void TuLuyen(object[] data)
        {
            tuLuyenCallback = data[0] as INetworkEvent.ITuLuyen;
            var time = (TimeSpan)data[1];
            Client.Instance.Send(MessageTag.TuLuyen, time);
        }

        private void UnEquipCongPhap(object[] data)
        {
            unequipCongPhapCallback = data[0] as INetworkEvent.IUnEquipCongPhap;
            var slot = (ETypeSlotCP)data[1];
            Client.Instance.Send(MessageTag.UnequipCongPhap, slot);
        }

        private void EquipCongPhap(object[] data)
        {
            equipCongPhapCallback = data[0] as INetworkEvent.IEquipCongPhap;
            var slot = (ETypeSlotCP)data[1];
            var id = (ushort)data[2];
            var request = new EquipCongPhap()
            {
                Slot = slot,
                Id = id,
            };
            Client.Instance.Send(MessageTag.EquipCongPhap, request);
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
            else if (tag == MessageTag.EquipCongPhapResult)
            {
                var result = reader.Read<EquipCongPhapResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.EquipmentManager.SetSlotCP(result.Slot, result.Id);
                    var cpRef = new CongPhap()
                    {
                        Id = result.Id,
                    };
                    if (!charData.InventoryManager.CongPhapOwners.TryGetValue(cpRef, out var congPhapOwner))
                    {
                        return;
                    }

                    congPhapOwner.GetCongPhap().EquipCongPhap(charData, congPhapOwner.Level);
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        equipCongPhapCallback.SuccessEquipCongPhap();
                    }
                    else
                    {
                        equipCongPhapCallback.FailEquipCongPhap(result.Message);
                    }
                });
            }
            else if (tag == MessageTag.UnequipCongPhapResult)
            {
                var result = reader.Read<UnequipCongPhapResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    var congPhapId = charData.EquipmentManager.GetSlotCP(result.Slot);
                    charData.EquipmentManager.SetSlotCP(result.Slot, ushort.MaxValue);
                    var cpRef = new CongPhap()
                    {
                        Id = congPhapId
                    };
                    if (!charData.InventoryManager.CongPhapOwners.TryGetValue(cpRef, out var congPhapOwner))
                    {
                        return;
                    }

                    congPhapOwner.GetCongPhap().UnEquipCongPhap(charData, congPhapOwner.Level);
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        unequipCongPhapCallback.SuccessUnEquipCongPhap();
                    }
                    else
                    {
                        unequipCongPhapCallback.FailUnEquipCongPhap(result.Message);
                    }
                });
            }
            else if (tag == MessageTag.TuLuyenResult)
            {
                var result = reader.Read<TuLuyenResult>();
                var charData = Client.Instance.CharacterData;
                if (result.Success)
                {
                    if (result.HasLevelUpdated)
                    {
                        result.LevelUpdated.ApplyTo(charData);
                    }
                    else
                    {
                        charData.Progression.CurrentExp = result.CurrentExp;
                    }
                    charData.Stats.LinhThach = result.RemainingMoney;
                }

                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        tuLuyenCallback.SuccessTuLuyen();
                    }
                    else
                    {
                        tuLuyenCallback.FailTuLuyen(result.Message);
                    }
                });
            }
            else if (tag == MessageTag.LinhNgoResult)
            {
                var result = reader.Read<LinhNgoResult>();
                if (result.Success)
                {
                    var type = result.Type;
                    var id = result.Id;
                    var itemRef = new ItemRef()
                    {
                        Id = id,
                        Type = type,
                    };
                    var remainingMoney = result.RemainingMoney;
                    var newTuoiTac = result.NewTuoiTac;
                    var charData = Client.Instance.CharacterData;
                    if (type == ItemType.CongPhap)
                    {
                        var congPhap = new CongPhap()
                        {
                            Id = id,
                            Level = 1
                        };
                        var inventory = charData.InventoryManager;
                        if (inventory.CongPhaps.TryGetValue(itemRef, out var cpRef))
                        {
                            cpRef.Count -= 1;
                            if (cpRef.Count < 1)
                            {
                                inventory.CongPhaps.Remove(itemRef);
                            }
                        }
                        inventory.Add(congPhap);
                    }
                    else if (type == ItemType.ThanThong)
                    {
                        var thanThong = new ThanThong()
                        {
                            Id = id,
                        };
                        var inventory = charData.InventoryManager;
                        if (inventory.ThanThongs.TryGetValue(itemRef, out var ttRef))
                        {
                            ttRef.Count -= 1;
                            if (ttRef.Count < 1)
                            {
                                inventory.ThanThongs.Remove(itemRef);
                            }
                        }
                        inventory.Add(thanThong);
                    }
                    charData.Stats.LinhThach = remainingMoney;
                    charData.Stats.TuoiTac = newTuoiTac;
                }
                NetworkManager.Instance.Invoke(() =>
                {
                    if (result.Success)
                    {
                        linhNgoCallback.SuccessLinhNgo();
                    }
                    else
                    {
                        linhNgoCallback.FailLinhNgo(result.Message);
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