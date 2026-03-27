using Base;
using System;
using System.Collections.Generic;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class CharacterData : IMemorySerializable<CharacterData>
    {
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public Gender Gender { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public int Level { get; set; } = 1;
        [field: SerializeField] public int CurrentExp { get; set; }
        [field: SerializeField] public CharStats.CharacterStats Stats { get; set; } = new CharStats.CharacterStats();
        public Dictionary<string, string> Skins { get; set; }
        public Dictionary<string, Color> Colors { get; set; }
        // ngoo dao
        public List<int> ngoDaoSaves { get; set; }// id
        public Dictionary<int, int> camNgoSaves { get; set; } // id va diem
        public int ngoDaoDiem { get; set; }

        // skill
        public List<int> thanThongOwner { get; set; } // id
        public List<int> thanThongEquip { get; set; }// id
        public Dictionary<int, int> congPhapOwner { get; set; } // id va level
        public int donThuatUsed { get; set; }
        public int phuTu1Used { get; set; }
        public int phuTu2Used { get; set; }
        public int phuTu3Used { get; set; }
        public int phuTu4Used { get; set; }
        public int chuTu1Used { get; set; }
        public int chuTu2Used { get; set; }
        // toong mon
        public EMonPhai monPhai;
        // DanDuoc
        public List<int> danCombatUsed { get; set; }// id



        public CharacterData()
        {
            
        }
        public CharacterData(CharacterData clone)
        {
            Id = clone.Id;
            Gender = clone.Gender;
            Name = clone.Name;
            Level = clone.Level;
            CurrentExp = clone.CurrentExp;
            Stats.CopyFrom(clone.Stats);
            Skins = new(clone.Skins);
            Colors = new(clone.Colors);
        }
        public int ByteSize()
        {
            return Id.ByteSize() 
                   + Gender.ByteSize() + Name.ByteSize() 
                   + Level.ByteSize() 
                   + CurrentExp.ByteSize()
                   + Stats.ByteSize() 
                   + Skins.Keys.ByteSize() + Skins.Values.ByteSize() 
                   + Colors.Keys.ByteSize() + Colors.Values.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Gender);
            writer.Write(Name);
            writer.Write(Level);
            writer.Write(CurrentExp);
            writer.Write(Stats);
            writer.WriteArray(Skins.Keys);
            writer.WriteArray(Skins.Values);
            writer.WriteArray(Colors.Keys);
            writer.WriteArray(Colors.Values);
        }

        public void Deserialize(IReader reader, ref CharacterData value)
        {
            value.Id = reader.Read<int>();
            value.Gender = reader.Read<Gender>();
            value.Name = reader.Read();
            value.Level = reader.Read<int>();
            value.CurrentExp = reader.Read<int>();
            value.Stats = reader.Read<CharStats.CharacterStats>();
            value.Skins = DictionaryUtils.CreateDictionary(reader.ReadArray(), reader.ReadArray());
            value.Colors = DictionaryUtils.CreateDictionary(reader.ReadArray(), reader.ReadArray<Color>());
        }
    }
}
