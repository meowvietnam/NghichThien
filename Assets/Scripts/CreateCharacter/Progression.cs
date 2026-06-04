using System;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    public class LevelUpdated : IMemorySerializable<LevelUpdated>
    {
        public int Level { get; set; }
        public int CurrentExp { get; set; }
        public int HpMax { get; set; }
        public int ThanThuc { get; set; }
        public int DonToc { get; set; }
        public int ThoNguyen { get; set; }

        public void Deserialize(IReader reader, ref LevelUpdated value)
        {
            value.Level = reader.Read<int>();
            value.CurrentExp = reader.Read<int>();
            value.HpMax = reader.Read<int>();
            value.ThanThuc = reader.Read<int>();
            value.DonToc = reader.Read<int>();
            value.ThoNguyen = reader.Read<int>();
        }

        public int ByteSize()
        {
            return Level.ByteSize() + CurrentExp.ByteSize() + HpMax.ByteSize() + ThanThuc.ByteSize()
                   + DonToc.ByteSize() + ThoNguyen.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Level);
            writer.Write(CurrentExp);
            writer.Write(HpMax);
            writer.Write(ThanThuc);
            writer.Write(DonToc);
            writer.Write(ThoNguyen);
        }

        public void ApplyTo(CharacterData charData)
        {
            var progression = charData.Progression;
            progression.Level = Level;
            progression.CurrentExp = CurrentExp;
            var stats = charData.Stats;
            stats.SinhMenh = HpMax;
            stats.ThanThuc = ThanThuc;
            stats.TocDoBay = DonToc;
            stats.ThoNguyen = ThoNguyen;
        }
    }
    [Serializable]
    public class Progression : IMemorySerializable<Progression>
    {
        [field: SerializeField] public int Level { get; set; }
        [field: SerializeField] public int CurrentExp { get; set; }
        [field: SerializeField] public int CurrentEnergy { get; set; }
        [field: SerializeField] public int NgoDaoDiem { get; set; }
        [field: SerializeField] public int IdCurrentNode { get; set; }
        
        public void Deserialize(IReader reader, ref Progression value)
        {
            value.Level = reader.Read<int>();
            value.CurrentExp = reader.Read<int>();
            value.CurrentEnergy = reader.Read<int>();
            value.NgoDaoDiem = reader.Read<int>();
            value.IdCurrentNode = reader.Read<int>();
        }

        public int ByteSize()
        {
            return Level.ByteSize() + CurrentExp.ByteSize() + CurrentEnergy.ByteSize() + NgoDaoDiem.ByteSize() + IdCurrentNode.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Level);
            writer.Write(CurrentExp);
            writer.Write(CurrentEnergy);
            writer.Write(NgoDaoDiem);
            writer.Write(IdCurrentNode);
        }
    }
}