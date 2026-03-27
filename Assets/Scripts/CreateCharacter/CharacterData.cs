using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using UnityEngine;
using EMonPhai = Base.EMonPhai;
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
        [field: SerializeField] public CharStats.CharacterStats Stats { get; set; } = new();
        public Dictionary<string, string> Skins { get; set; } = new();
        public Dictionary<string, Color> Colors { get; set; } = new();
        public EMonPhai MonPhai { get; set; }
        public List<int> NgoDaoSaves { get; set; } = new();
        public Dictionary<int, int> CamNgoSaves { get; set; } = new();
        public int NgoDaoDiem { get; set; }
        public List<int> ThanThongOwner { get; set; } = new();
        public List<int> ThanThongEquip { get; set; } = new();
        public Dictionary<int, int> CongPhapOwner { get; set; } = new();
        public int DonThuatUsed { get; set; }
        public int PhuTu1Used { get; set; }
        public int PhuTu2Used { get; set; }
        public int PhuTu3Used { get; set; }
        public int PhuTu4Used { get; set; }
        public int ChuTu1Used { get; set; }
        public int ChuTu2Used { get; set; }
        public List<int> DanCombatUsed { get; set; } = new();

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
            MonPhai = clone.MonPhai;
            NgoDaoSaves = clone.NgoDaoSaves;
            CamNgoSaves = clone.CamNgoSaves;
            NgoDaoDiem = clone.NgoDaoDiem;
            ThanThongOwner = clone.ThanThongOwner;
            ThanThongEquip = clone.ThanThongEquip;
            CongPhapOwner = new(clone.CongPhapOwner);
            DonThuatUsed = clone.DonThuatUsed;
            PhuTu1Used = clone.PhuTu1Used;
            PhuTu2Used = clone.PhuTu2Used;
            PhuTu3Used = clone.PhuTu3Used;
            PhuTu4Used = clone.PhuTu4Used;
            ChuTu1Used = clone.ChuTu1Used;
            ChuTu2Used = clone.ChuTu2Used;
            DanCombatUsed = new(clone.DanCombatUsed);
        }

        public int ByteSize()
        {
            return Id.ByteSize()
                   + Gender.ByteSize() + Name.ByteSize()
                   + Level.ByteSize()
                   + CurrentExp.ByteSize()
                   + Stats.ByteSize()
                   + Skins.Keys.ByteSize() + Skins.Values.ByteSize()
                   + Colors.Keys.ByteSize() + Colors.Values.ByteSize()
                   + MonPhai.ByteSize() + NgoDaoSaves.ByteSize()
                   + CamNgoSaves.Keys.ByteSize() + CamNgoSaves.Values.ByteSize()
                   + NgoDaoDiem.ByteSize() + ThanThongOwner.ByteSize() + ThanThongEquip.ByteSize()
                   + CongPhapOwner.Keys.ByteSize() + CongPhapOwner.Values.ByteSize()
                   + DonThuatUsed.ByteSize()
                   + PhuTu1Used.ByteSize() + PhuTu2Used.ByteSize()
                   + PhuTu3Used.ByteSize() + PhuTu4Used.ByteSize()
                   + ChuTu1Used.ByteSize() + ChuTu2Used.ByteSize()
                   + DanCombatUsed.ByteSize();
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
            writer.Write(MonPhai);
            writer.WriteArray(NgoDaoSaves);
            writer.WriteArray(CamNgoSaves.Keys);
            writer.WriteArray(CamNgoSaves.Values);
            writer.Write(NgoDaoDiem);
            writer.WriteArray(ThanThongOwner);
            writer.WriteArray(ThanThongEquip);
            writer.WriteArray(CongPhapOwner.Keys);
            writer.WriteArray(CongPhapOwner.Values);
            writer.Write(DonThuatUsed);
            writer.Write(PhuTu1Used);
            writer.Write(PhuTu2Used);
            writer.Write(PhuTu3Used);
            writer.Write(PhuTu4Used);
            writer.Write(ChuTu1Used);
            writer.Write(ChuTu2Used);
            writer.WriteArray(DanCombatUsed);
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
            value.MonPhai = reader.Read<EMonPhai>();
            value.NgoDaoSaves = reader.ReadArray<int>().ToList();
            value.CamNgoSaves = DictionaryUtils.CreateDictionary(reader.ReadArray<int>(), reader.ReadArray<int>());
            value.NgoDaoDiem = reader.Read<int>();
            value.ThanThongOwner = reader.ReadArray<int>().ToList();
            value.ThanThongEquip = reader.ReadArray<int>().ToList();
            value.CongPhapOwner = DictionaryUtils.CreateDictionary(reader.ReadArray<int>(), reader.ReadArray<int>());
            value.DonThuatUsed = reader.Read<int>();
            value.PhuTu1Used = reader.Read<int>();
            value.PhuTu2Used = reader.Read<int>();
            value.PhuTu3Used = reader.Read<int>();
            value.PhuTu4Used = reader.Read<int>();
            value.ChuTu1Used = reader.Read<int>();
            value.ChuTu2Used = reader.Read<int>();
            value.DanCombatUsed = reader.ReadArray<int>().ToList();
        }
    }
}