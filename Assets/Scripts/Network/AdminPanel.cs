using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using LuyenKhi;
using Network.ClientState;
using Network.Core;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using Sirenix.OdinInspector;
using Tutorials;
using UnityEngine;
using CongPhap = Skill.CongPhap;
using ThanThong = Skill.ThanThong;

namespace Network
{
    public class AdminPanel : MonoBehaviour, IClientHandler
    {
        private Client Client => Client.Instance;
        private static AdminPanel instance;
#if UNITY_EDITOR
        private string secretKey = "qnn22n2us7g2bbna-aql11.e22111m1m";
#else
        private string secretKey = "";
#endif
        public static AdminPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<AdminPanel>();
                }

                return instance;
            }
        }

        private bool IsReady()
        {
            if (Client == null)
            {
                return false;
            }

            return Client.CharacterData != null;
        }

        [Button]
        public void LogArena()
        {
            Debug.Log("CurrentCount " + Client.CharacterData.ArenaManager.CurrentArenaCount);

            Debug.Log("Arena Poin " + Client.CharacterData.ArenaManager.Points);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        public void AddItems(List<ItemRef> items)
        {
            if (items == null || items.Count == 0)
            {
                Debug.LogWarning("AddItems called with no items");
                return;
            }

            Client.Send(MessageTag.AddItems, items);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        public void AddFullItem()
        {
            var gameConfig = Pool.Get<GameManager>();
            List<ItemRef> items = new List<ItemRef>();
            foreach (var item in gameConfig.thanThongs)
            {
                items.Add(new ItemRef
                {
                    Id = (ushort)item,
                    Count = 1,
                    Type = ItemType.ThanThong
                });
            }

            foreach (var item in gameConfig.congPhaps)
            {
                items.Add(new ItemRef
                {
                    Id = (ushort)item,
                    Count = 1,
                    Type = ItemType.CongPhap
                });
            }

            foreach (var item in LuyenDanResources.Instance.Recipes)
            {
                items.Add(new ItemRef
                {
                    Id = (ushort)item.Id,
                    Count = 1,
                    Type = ItemType.Recipe
                });
            }

            foreach (var item in LuyenDanResources.Instance.HerbDatas)
            {
                items.Add(new ItemRef
                {
                    Id = (ushort)item.Id,
                    Count = 1,
                    Type = ItemType.Herb
                });
            }

            foreach (var item in LuyenDanResources.Instance.TempFurnaceDatas)
            {
                items.Add(new ItemRef
                {
                    Id = (ushort)item.Id,
                    Count = 1,
                    Type = ItemType.Furnace
                });
            }

            var luyenKhiRes = LuyenKhiResources.Instance;
            var mats = luyenKhiRes.Materials.Select(x => new ItemRef()
            {
                Id = (ushort)x.Id,
                Count = 20,
                Type = ItemType.Material
            });
            items.AddRange(mats);
            AddItems(items);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        public void SetInfinityEnergy(bool isInfinityEnergy)
        {
            Client.Send(MessageTag.SetInfinityEnergy, isInfinityEnergy);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        public void SetNgoDaoDiem(int point)
        {
            Client.Send(MessageTag.SetNgoDaoDiem, point);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        public void SetCamNgo(CamNgo camNgo)
        {
            Client.Send(MessageTag.SetCamNgo, camNgo);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        public void AddThanThong(ThanThong thanThong)
        {
            Client.Send(MessageTag.AddThanThong, (ushort)thanThong.Id);
        }

        public void StartListen()
        {
            var module = Client.CoreModule;
            module.RemoveHandler<AdminPanel>();
            module.AddHandler(this);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        public void AddCongPhap(CongPhap congPhap, ushort level)
        {
            if (level == 0) return;
            var cp = new CreateCharacter.CongPhap()
            {
                Id = (ushort)congPhap.Id,
                Level = level,
            };
            Client.Send(MessageTag.AddCongPhap, cp);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        public void SetMoney(int money)
        {
            Client.Send(MessageTag.SetMoney, money);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        public void SetMoneyVip(int money)
        {
            Client.Send(MessageTag.SetMoneyVip, money);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        private async void SetExp(int exp)
        {
            var reader = await Client.RequestWait(MessageTag.SetExpRequest, exp, 20000);
            var currentExp = reader.Read<int>();
            Client.CharacterData.Progression.CurrentExp = currentExp;
        }

        [Button]
        [ShowIf(nameof(IsAuthStateAndNotLogin))]
        private void AdminLoginGoogle(string email)
        {
            var state = Client.State as AuthState;
            state?.Login(new AdminLoginWithGoogle()
            {
                Email = email,
                SecretKey = secretKey
            });
        }

        [Button]
        [ShowIf(nameof(IsAuthStateAndNotLogin))]
        private void AdminLoginWithName(string name)
        {
            var state = Client.State as AuthState;
            state?.Login(new AdminLoginWithName()
            {
                Name = name,
                SecretKey = secretKey
            });
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        private void SendNotify(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }

            Client.Send(MessageTag.AdminNotify, msg);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        private async void CreateAuctionSession(CreateNewAuctionSession session)
        {
            var reader = await Client.MemRequestWait(MessageTag.CreateNewAuctionSessionRequest, session);
            var result = reader.MemRead<Result>();
            NotifyGroup.Instance?.Show(result.Message);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        private async void RemoveLastAuctionSession()
        {
            var reader = await Client.RequestWait(MessageTag.RemoveLastAuctionSessionRequest);
            var result = reader.MemRead<Result>();
            NotifyGroup.Instance?.Show(result.Message);
        }
        
        private bool IsAuthStateAndNotLogin()
        {
            if (Client == null)
            {
                return false;
            }

            if (Client.UserData != null)
            {
                return false;
            }

            return Client.State is AuthState;
        }

        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.AddItemsResult)
            {
                var rewards = reader.Read<RewardItems>();
                var inventoryManager = Client.CharacterData.InventoryManager;
                inventoryManager.AddToInventory(rewards);
            }
            else if (tag == MessageTag.SetNgoDaoDiemResult)
            {
                var ngoDaoDiem = reader.Read<int>();
                Client.CharacterData.Progression.NgoDaoDiem = ngoDaoDiem;
            }
            else if (tag == MessageTag.SetCamNgoResult)
            {
                var result = reader.Read<SetCamNgoResult>();
                var charData = Client.CharacterData;
                charData.InventoryManager.AddOrUpdate(result.CamNgo);
                charData.Progression.NgoDaoDiem = result.NewNgoDaoDiem;
            }
            else if (tag == MessageTag.AddThanThongResult)
            {
                var id = reader.Read<ushort>();
                Client.CharacterData.InventoryManager.EquipThanThong(id);
            }
            else if (tag == MessageTag.AddCongPhapResult)
            {
                var congPhap = reader.Read<CreateCharacter.CongPhap>();
                Client.CharacterData.InventoryManager.AddOrUpdate(congPhap);
            }
            else if (tag == MessageTag.SetMoneyVipResult)
            {
                var money = reader.Read<int>();
                Client.CharacterData.MoneyVip = money;
            }
            else if (tag == MessageTag.SetMoneyResult)
            {
                var money = reader.Read<int>();
                Client.CharacterData.Stats.LinhThach = money;
            }
        }

        [Button]
        public void RenewTutorial()
        {
            PlayerPrefs.DeleteKey(TutorialAllMaps.tutorialAllMapKey);
            PlayerPrefs.DeleteKey(TutorialBeQuan.tutorialBeQuan);
            PlayerPrefs.DeleteKey(TutorialHome.tutorialHome);
            PlayerPrefs.DeleteKey(TutorialBoCao.tutorialBoCao);
            PlayerPrefs.DeleteKey(TutorialLuyenDan.tutorialLuyenDan);
            PlayerPrefs.Save();
            Debug.Log("Renew all tutorial");
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handler =
        {
            MessageTag.AddItemsResult, MessageTag.SetNgoDaoDiemResult, MessageTag.SetCamNgoResult,
            MessageTag.AddThanThongResult, MessageTag.AddCongPhapResult, MessageTag.SetMoneyVipResult,
            MessageTag.SetMoneyResult
        };

        public MessageTag[] HandleTag => handler;
    }
}