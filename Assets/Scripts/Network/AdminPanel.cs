using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenKhi;
using Network.Core;
using Network.Extensions;
using Sirenix.OdinInspector;
using Skill;
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
        private HashSet<ThanThongEquip> GetFullIdThanThongWithCombo(ComboSkillResource.ETypeCombo eCombo)
        {
            // 1. Tìm combo
            var combo = ComboSkillResource.Instance.comboNPC.FirstOrDefault(x => x.typeCombo == eCombo);
            if (combo == null) return new HashSet<ThanThongEquip>();

            // 2. Tính toán Index đại cảnh giới
            int daiCanhGioiIndex = Skill.ThanThong.GetDaiCanhGioi(Client.CharacterData.Progression.Level) - 1;

            // SỬA LỖI: Thay .Count bằng .Length vì đây là Mảng (Array)
            if (daiCanhGioiIndex < 0 || daiCanhGioiIndex >= combo.comboWithDaiCanhGioi.Length)
                return new HashSet<ThanThongEquip>();

            // 3. Lấy danh sách ThanThong nguồn
            var listThanThong = combo.comboWithDaiCanhGioi[daiCanhGioiIndex].thanThongs;

            // 4. Convert sang HashSet<ThanThongEquip>
            var result = listThanThong.Select(x => new ThanThongEquip
            {
                // SỬA LỖI: Ép kiểu từ int sang ushort để khớp với ThanThongEquip
                Id = (ushort)x.Id,
                // Các trường khác nếu cần...
            }).ToHashSet();

            return result;
        }

        [Button]
        private List<int> GetFullIdNgoDaoWithCombo(ComboSkillResource.ETypeCombo eCombo)
        {
            var combo = ComboSkillResource.Instance.comboNPC.FirstOrDefault(x => x.typeCombo == eCombo);
            return combo
                .comboWithDaiCanhGioi[Skill.ThanThong.GetDaiCanhGioi(Client.CharacterData.Progression.Level) - 1]
                .ngoDaos.Select(x => x.Id) // Giả sử trong class ThanThong của bạn biến là ID hoặc id
                .ToList();
        }

        [Button]
        private List<int> GetFullIdCongPhapWithCombo(ComboSkillResource.ETypeCombo eCombo)
        {
            var combo = ComboSkillResource.Instance.comboNPC.FirstOrDefault(x => x.typeCombo == eCombo);
            return combo
                .comboWithDaiCanhGioi[Skill.ThanThong.GetDaiCanhGioi(Client.CharacterData.Progression.Level) - 1]
                .congPhaps.Select(x => x.Id) // Giả sử trong class ThanThong của bạn biến là ID hoặc id
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
        [ShowIf(nameof(IsReady))]
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

        public void StartListen()
        {
            var module = Client.CoreModule;
            module.RemoveHandler<AdminPanel>();
            module.AddHandler(this);
        }

        [Button]
        [ShowIf(nameof(IsReady))]
        public void ToLuyenKhi()
        {
            Observer.Notify(EventList.GamePlay.LOAD_SCENE, "LuyenKhi");
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
                var camNgo = reader.Read<CamNgo>();
                Client.CharacterData.InventoryManager.AddOrUpdate(camNgo);
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handler =
            { MessageTag.AddItemsResult, MessageTag.SetNgoDaoDiemResult, MessageTag.SetCamNgoResult };

        public MessageTag[] HandleTag => handler;
    }
}