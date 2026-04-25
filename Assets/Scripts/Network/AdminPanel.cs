using System.Collections.Generic;
using System.Linq;
using LongDH.LongDHModules;
using LuyenDan;
using Network.Core;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Network
{
    public class AdminPanel : MonoBehaviour, IClientHandler
    {
        private Client Client => Client.Instance;
        private static AdminPanel instance;

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
        private List<int> GetFullIdThanThongWithCombo(ComboSkillResource.ETypeCombo eCombo)
        {
            var combo = ComboSkillResource.Instance.comboNPC.FirstOrDefault(x => x.typeCombo == eCombo);
            return combo.comboWithDaiCanhGioi[ThanThong.GetDaiCanhGioi(Client.CharacterData.Level) - 1].thanThongs.Select(x => x.Id) // Giả sử trong class ThanThong của bạn biến là ID hoặc id
                .ToList();
        }
        [Button]
        private List<int> GetFullIdNgoDaoWithCombo(ComboSkillResource.ETypeCombo eCombo)
        {
            var combo = ComboSkillResource.Instance.comboNPC.FirstOrDefault(x => x.typeCombo == eCombo);
            return combo.comboWithDaiCanhGioi[ThanThong.GetDaiCanhGioi(Client.CharacterData.Level) - 1].ngoDaos.Select(x => x.Id) // Giả sử trong class ThanThong của bạn biến là ID hoặc id
                .ToList();
        }
        [Button]
        private List<int> GetFullIdCongPhapWithCombo(ComboSkillResource.ETypeCombo eCombo)
        {
            var combo = ComboSkillResource.Instance.comboNPC.FirstOrDefault(x => x.typeCombo == eCombo);
            return combo.comboWithDaiCanhGioi[ThanThong.GetDaiCanhGioi(Client.CharacterData.Level) - 1].congPhaps.Select(x => x.Id) // Giả sử trong class ThanThong của bạn biến là ID hoặc id
                .ToList();
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
        public void AddFullItem()
        {
            var gameConfig = Pool.Get<GameManager>();
            List<ItemRef> items = new List<ItemRef>();
            foreach (var item in gameConfig.thanThongs)
            {
                items.Add(new ItemRef
                {
                    Id = (ushort)item.Id,
                    Count = 1,
                    Type = ItemType.ThanThong
                });
            }
            foreach (var item in gameConfig.congPhaps)
            {
                items.Add(new ItemRef
                {
                    Id = (ushort)item.Id,
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
            AddItems(items);
        }
        public void StartListen()
        {
            var module = Client.CoreModule;
            module.RemoveHandler<AdminPanel>();
            module.AddHandler(this);
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
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handler = { MessageTag.AddItemsResult };
        public MessageTag[] HandleTag => handler;
    }
}