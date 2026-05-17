using System;
using System.IO;
using System.Linq;
using Base;
using LuyenDan.Inventory;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class HerbData : IItemData, IMemorySerializable<HerbData>
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string ChuDuoc { get; private set; }
        [field: SerializeField] public string PhuDuoc { get; private set; }
        [field: SerializeField] public string ThuocDan { get; private set; }
        [field: SerializeField] public string Locations { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        private Sprite icon;
        [field: SerializeField] public string IconPath { get; private set; }
        public Sprite Icon
        {
            get
            {
                if (icon == null)
                {
                    icon = Resources.Load<Sprite>(IconPath);
                }
                return icon;
            }
        }
        [ShowInInspector]
        public HerbType HerbType
        {
            get
            {
                var type = HerbType.None;
                if (!string.IsNullOrWhiteSpace(ChuDuoc))
                {
                    type |= HerbType.ChuDuoc;
                }

                if (!string.IsNullOrWhiteSpace(PhuDuoc))
                {
                    type |= HerbType.PhuDuoc;
                }

                if (!string.IsNullOrWhiteSpace(ThuocDan))
                {
                    type |= HerbType.DuocDan;
                }

                return type;
            }
        }

        public HerbData(int id, Tier tier, string name, string chuduoc, string phuduoc, string thuocdan,
            string locations)
        {
            Id = id;
            Tier = tier;
            Name = name;
            ChuDuoc = chuduoc;
            PhuDuoc = phuduoc;
            ThuocDan = thuocdan;
            Locations = locations;
        }

        public HerbData()
        {
        }

        public void Modify()
        {
            Name = Normalize1(Name);
            ChuDuoc = Normalize2(ChuDuoc);
            PhuDuoc = Normalize2(PhuDuoc);
            ThuocDan = Normalize2(ThuocDan);
            Locations = Locations.Trim();
        }

        private string Normalize(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Substring(0, 1).ToUpper() + text.Substring(1).ToLower();
        }

        private string Normalize1(string text)
        {
            text = text.Trim();
            var parts = text.Split(" ");
            return string.Join(" ", parts.Select(Normalize));
        }

        private string Normalize2(string text)
        {
            text = text.Trim();
            return Normalize(text);
        }

        public bool Is(HerbType type)
        {
            var myType = HerbType;
            return myType.HasFlag(type);
        }

        public int ByteSize()
        {
            return Id.ByteSize() + Tier.ByteSize() + Name.ByteSize()
                   + ChuDuoc.ByteSize() + PhuDuoc.ByteSize() + ThuocDan.ByteSize()
                   + Locations.ByteSize() + Description.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Tier);
            writer.Write(Name);
            writer.Write(ChuDuoc);
            writer.Write(PhuDuoc);
            writer.Write(ThuocDan);
            writer.Write(Locations);
            writer.Write(Description);
        }

        public void Deserialize(IReader reader, ref HerbData value)
        {
            value.Id = reader.Read<int>();
            value.Tier = reader.Read<Tier>();
            value.Name = reader.Read();
            value.ChuDuoc = reader.Read();
            value.PhuDuoc = reader.Read();
            value.ThuocDan = reader.Read();
            value.Locations = reader.Read();
            value.Description = reader.Read();
        }
    }
}