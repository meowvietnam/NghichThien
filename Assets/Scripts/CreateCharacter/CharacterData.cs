using Base;
using System;
using AllMaps;
using AllMaps.EventReward;
using CreateCharacter.CharStats;
using LongDH.LongDHModules;
using MemoryPack;
using Network;
using Network.Extensions;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using EMonPhai = Base.EMonPhai;

namespace CreateCharacter
{
    [Serializable]
    [MemoryPackable]
    public sealed partial class CharacterData
    {
        [field: SerializeField, TabGroup("Basic")]
        public int Id { get; set; }

        [field: SerializeField, TabGroup("Basic")]
        public Gender Gender { get; set; }

        [field: SerializeField, TabGroup("Basic")]
        public string Name { get; set; }

        [field: SerializeField, TabGroup("Basic")]
        public EMonPhai MonPhai { get; set; }

        [field: SerializeField, TabGroup("Basic")]
        public int MoneyVip { get; set; }

        [field: SerializeField, TabGroup("Basic")]
        public bool IsInfiniteEnergy { get; set; }

        [field: SerializeField, TabGroup("Basic")]
        public bool IsNoAds { get; set; }

        [field: ShowInInspector, TabGroup("Basic")]
        public bool CanFly { get; set; }

        [field: ShowInInspector, TabGroup("Basic")]
        public int CurrentFreeAdsMoneyVipCount { get; set; }

        [field: ShowInInspector, TabGroup("Basic")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [field: ShowInInspector, TabGroup("Progression")]
        public Progression Progression { get; set; } = new();

        [field: SerializeField, TabGroup("Stats")]
        public CharacterStats Stats { get; set; } = new();

        [field: SerializeField, TabGroup("Appearance")]
        public Appearance Appearance { get; set; } = new();

        [field: SerializeField, TabGroup("Inventory")]
        public InventoryManager InventoryManager { get; private set; } = new();

        [field: SerializeField, TabGroup("EquipmentManager")]
        public EquipmentManager EquipmentManager { get; set; } = new();

        [field: SerializeField, TabGroup("Social")]
        public Social Social { get; set; } = new();

        [field: SerializeField, TabGroup("Quest")]
        public QuestManager QuestManager { get; set; } = new();

        [field: SerializeField, TabGroup("Arena")]
        public ArenaManager ArenaManager { get; set; } = new();

        [field: SerializeField, TabGroup("ShopManager")]
        public ShopManager ShopManager { get; set; } = new();

        [field: SerializeField, TabGroup("GachaManager")]
        public GachaManager GachaManager { get; set; } = new();

        public event Action<InventoryManager> OnInventoryUpdated;

        public static void Sync(CharacterData source, CharacterData target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.SyncFrom(source);
        }

        public void SyncFrom(CharacterData source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var synced = source.DeepClone();
            Id = synced.Id;
            Gender = synced.Gender;
            Name = synced.Name;
            MonPhai = synced.MonPhai;
            MoneyVip = synced.MoneyVip;
            IsInfiniteEnergy = synced.IsInfiniteEnergy;
            IsNoAds = synced.IsNoAds;
            CanFly = synced.CanFly;
            CurrentFreeAdsMoneyVipCount = synced.CurrentFreeAdsMoneyVipCount;
            CreatedAt = synced.CreatedAt;
            Progression = synced.Progression ?? new Progression();
            Stats ??= new CharacterStats();
            Stats.CopyFrom(synced.Stats ?? new CharacterStats());
            Appearance = synced.Appearance ?? new Appearance();
            InventoryManager = synced.InventoryManager ?? new InventoryManager();
            EquipmentManager = synced.EquipmentManager ?? new EquipmentManager();
            Social = synced.Social ?? new Social();
            QuestManager = synced.QuestManager ?? new QuestManager();
            ArenaManager = synced.ArenaManager ?? new ArenaManager();
            ShopManager = synced.ShopManager ?? new ShopManager();
            GachaManager = synced.GachaManager ?? new GachaManager();
            InventoryChanged();
            Observer.Notify(EventList.GamePlay.UPDATE_MAILS);
        }

        public CharacterData DeepClone()
        {
            return MemoryPackSerializer.Deserialize<CharacterData>(MemoryPackSerializer.Serialize(this));
        }

        public void SetInventory(InventoryManager inventory)
        {
            InventoryManager = inventory;
            InventoryChanged();
        }

        public void InventoryChanged()
        {
            OnInventoryUpdated?.Invoke(InventoryManager);
        }

        public void GetInventory()
        {
            Client.Instance.Send(MessageTag.GetInventory);
        }

        public void ApplyInventoryModify(InventoryModify modify)
        {
            InventoryManager.ApplyModify(modify);
            InventoryChanged();
        }

        public void ApplyBuff(NgoDaoBuff buff)
        {
            var type = buff.BuffStat;
            var buffValue = buff.BuffValue;
            switch (type)
            {
                case NgoDaoBuffStats.EBuffStats.ThanThuc:
                    Stats.ThanThuc += buffValue;
                    break;
                case NgoDaoBuffStats.EBuffStats.HP:
                    Stats.SinhMenh += buffValue;
                    Stats.Hp += buffValue;
                    break;
                case NgoDaoBuffStats.EBuffStats.DonToc:
                    Stats.TocDoBay += buffValue;
                    break;
            }
        }

        public void EquipThanThong(EquipThanThongResult result)
        {
            var thanThong = result.ThanThong;
            EquipmentManager.EquipThanThong(thanThong);
        }

        public void ApplyDailyReset(DailyReset dailyReset)
        {
            ArenaManager.CurrentArenaCount = dailyReset.CurrentArenaCount;
            QuestManager.CurrentCountSideQuest = dailyReset.CurrentCountSideQuest;
        }

        public void Claim(RewardSnapshot reward)
        {
            MoneyVip += reward.MoneyVip;
            Stats.LinhThach += reward.Money;
            if (reward.RewardItems != null)
            {
                InventoryManager.AddToInventory(reward.RewardItems);
            }
        }
    }
}
