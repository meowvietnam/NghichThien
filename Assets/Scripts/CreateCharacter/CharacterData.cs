using Base;
using System;
using CreateCharacter.CharStats;
using Network;
using Network.Extensions;
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

        [field: ShowInInspector, TabGroup("Basic")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [field: ShowInInspector, TabGroup("Progression")]
        public Progression Progression { get; set; } = new();

        [field: SerializeField, TabGroup("Stats")]
        public CharacterStats Stats { get; set; } = new();


        [field: SerializeField, TabGroup("Appearance")]
        public Appearance Appearance { get; set; } = new();

        [field: SerializeField, TabGroup("Inventory")]
        public InventoryManager InventoryManager { get; set; } = new();
        [field: SerializeField, TabGroup("Equipment")]
        public Equipment Equipment { get; set; } = new();

        [field: SerializeField, TabGroup("Social")]
        public Social Social { get; set; } = new();

        public CharacterData()
        {
        }

        public CharacterData(CharacterData clone)
        {
            Id = clone.Id;
            Gender = clone.Gender;
            Name = clone.Name;
            MonPhai = clone.MonPhai;
            CreatedAt = clone.CreatedAt;
        }

        public int ByteSize()
        {
            return Id.ByteSize()
                   + Gender.ByteSize() + Name.ByteSize()
                   + Stats.ByteSize()
                   + Progression.ByteSize()
                   + Appearance.ByteSize()
                   + Equipment.ByteSize()
                   + MonPhai.ByteSize()
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
            writer.Write(Equipment);
            writer.Write(MonPhai);
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
            value.Equipment = reader.Read<Equipment>();
            value.MonPhai = reader.Read<EMonPhai>();
            value.InventoryManager = reader.Read<InventoryManager>();
            value.CreatedAt = reader.Read<DateTime>();
            value.Social = reader.Read<Social>();
        }
    }
}