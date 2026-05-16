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
            CreatedAt = clone.CreatedAt;
        }

        public int ByteSize()
        {
            return Id.ByteSize()
                   + Gender.ByteSize() + Name.ByteSize()
                   + Stats.ByteSize()
                   + Progression.ByteSize()
                   + Appearance.ByteSize()
                   + EquipmentManager.ByteSize()
                   + MonPhai.ByteSize()
                   + MoneyVip.ByteSize()
                   + InventoryManager.ByteSize()
                   + CreatedAt.ByteSize()
                   + Social.ByteSize();
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
            writer.Write(InventoryManager);
            writer.Write(CreatedAt);
            writer.Write(Social);
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
            value.InventoryManager = reader.Read<InventoryManager>();
            value.CreatedAt = reader.Read<DateTime>();
            value.Social = reader.Read<Social>();
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
                    break;
                case NgoDaoBuffStats.EBuffStats.DonToc:
                    Stats.TocDoBay += buffValue;
                    break;
            }
        }
    }
}