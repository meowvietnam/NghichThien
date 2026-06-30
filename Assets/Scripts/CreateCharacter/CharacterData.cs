using Base;
using System;
using CreateCharacter.CharStats;
using Network;
using Network.Extensions;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using EMonPhai = Base.EMonPhai;

namespace CreateCharacter
{
    [Serializable]
    public sealed class CharacterData : IMemorySerializable<CharacterData>
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
        public int CurrentArenaCount { get; set; }

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

        public event Action<InventoryManager> OnInventoryUpdated;

        public CharacterData()
        {
        }

        public CharacterData(CharacterData clone)
        {
            Id = clone.Id;
            Gender = clone.Gender;
            Name = clone.Name;
            MonPhai = clone.MonPhai;
            MoneyVip = clone.MoneyVip;
            IsInfiniteEnergy = clone.IsInfiniteEnergy;
            IsNoAds = clone.IsNoAds;
            CanFly = clone.CanFly;
            CurrentArenaCount = clone.CurrentArenaCount;
            CurrentFreeAdsMoneyVipCount = clone.CurrentFreeAdsMoneyVipCount;
            CreatedAt = clone.CreatedAt;
        }

        public int ByteSize()
        {
            return Id.ByteSize()
                   + Gender.ByteSize()
                   + Name.ByteSize()
                   + Stats.ByteSize()
                   + Progression.ByteSize()
                   + Appearance.ByteSize()
                   + EquipmentManager.ByteSize()
                   + MonPhai.ByteSize()
                   + MoneyVip.ByteSize()
                   + IsInfiniteEnergy.ByteSize()
                   + IsNoAds.ByteSize()
                   + CanFly.ByteSize()
                   + CurrentArenaCount.ByteSize()
                   + CurrentFreeAdsMoneyVipCount.ByteSize()
                   + InventoryManager.ByteSize()
                   + CreatedAt.ByteSize()
                   + Social.ByteSize()
                   + QuestManager.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Gender);
            writer.Write(Name);
            writer.Write(Stats);
            writer.Write(Progression);
            writer.Write(Appearance);
            writer.Write(EquipmentManager);
            writer.Write(MonPhai);
            writer.Write(MoneyVip);
            writer.Write(IsInfiniteEnergy);
            writer.Write(IsNoAds);
            writer.Write(CanFly);
            writer.Write(CurrentArenaCount);
            writer.Write(CurrentFreeAdsMoneyVipCount);
            writer.Write(InventoryManager);
            writer.Write(CreatedAt);
            writer.Write(Social);
            writer.Write(QuestManager);
        }

        public void Deserialize(IReader reader, ref CharacterData value)
        {
            value.Id = reader.Read<int>();
            value.Gender = reader.Read<Gender>();
            value.Name = reader.Read();
            value.Stats = reader.Read<CharacterStats>();
            value.Progression = reader.Read<Progression>();
            value.Appearance = reader.Read<Appearance>();
            value.EquipmentManager = reader.Read<EquipmentManager>();
            value.MonPhai = reader.Read<EMonPhai>();
            value.MoneyVip = reader.Read<int>();
            value.IsInfiniteEnergy = reader.Read<bool>();
            value.IsNoAds = reader.Read<bool>();
            value.CanFly = reader.Read<bool>();
            value.CurrentArenaCount = reader.Read<int>();
            value.CurrentFreeAdsMoneyVipCount = reader.Read<int>();
            value.InventoryManager = reader.Read<InventoryManager>();
            value.CreatedAt = reader.Read<DateTime>();
            value.Social = reader.Read<Social>();
            value.QuestManager = reader.Read<QuestManager>();
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
    }
}