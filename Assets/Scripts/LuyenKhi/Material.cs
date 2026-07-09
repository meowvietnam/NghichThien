using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base;
using LuyenDan.Inventory;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class Material : IItemData, IMemorySerializable<Material>
    {
        [field: SerializeField, TabGroup("Info")]
        public int Id { get; set; }

        [field: SerializeField, TabGroup("Info")]
        public string Name { get; set; }

        [field: SerializeField, TabGroup("Info")]
        public Tier Tier { get; set; }

        private Sprite icon;

        [field: SerializeField, TabGroup("Info")]
        public string IconPath { get; set; }

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

        [field: SerializeField, TabGroup("Info")]
        public CultivationElement CultivationElement { get; set; }

        [field: SerializeField, TabGroup("Info")]
        public YinYangType YinYangType { get; set; }

        public string Description => Desc2;

        [field: SerializeField, TabGroup("Ref")]
        public int TypeRef { get; set; }

        [field: SerializeField, TabGroup("Ref")]
        public int PropertyRef { get; set; }

        [field: SerializeField, TabGroup("Ref")]
        public int[] OriginRef { get; set; }

        [field: SerializeField, TabGroup("Ref")]
        public int Desc1Ref { get; set; }

        [field: SerializeField, TabGroup("Ref")]
        public int Desc2Ref { get; set; }

        [field: SerializeField, TabGroup("Data")]
        public int[] ItemFlags { get; set; }

        [field: SerializeField, TabGroup("Data")]
        public int Price { get; set; }

        private StringDict Dict => LuyenKhiResources.Instance.Strings;
        [TabGroup("Preview"), ShowInInspector] public string Type => Dict[TypeRef];
        [TabGroup("Preview"), ShowInInspector] public string Property => Dict[PropertyRef];
        [TabGroup("Preview"), ShowInInspector] public string[] Origin => OriginRef.Select(o => Dict[o]).ToArray();
        [TabGroup("Preview"), ShowInInspector] public string Desc1 => Dict[Desc1Ref];
        [TabGroup("Preview"), ShowInInspector] public string Desc2 => Dict[Desc2Ref];

        public void Deserialize(IReader reader, ref Material value)
        {
            value.Id = reader.Read<int>();
            value.Name = reader.Read();
            value.Tier = reader.Read<Tier>();
            value.CultivationElement = reader.Read<CultivationElement>();
            value.YinYangType = reader.Read<YinYangType>();
            value.TypeRef = reader.Read<int>();
            value.PropertyRef = reader.Read<int>();
            value.OriginRef = reader.ReadArray<int>();
            value.Desc1Ref = reader.Read<int>();
            value.Desc2Ref = reader.Read<int>();
            value.ItemFlags = reader.ReadArray<int>();
            value.Price = reader.Read<int>();
        }

        public int ByteSize()
        {
            return Id.ByteSize() + Name.ByteSize() + Tier.ByteSize()
                   + CultivationElement.ByteSize() + YinYangType.ByteSize() + TypeRef.ByteSize()
                   + PropertyRef.ByteSize() + OriginRef.ByteSize() + Desc1Ref.ByteSize()
                   + Desc2Ref.ByteSize() + ItemFlags.ByteSize() + Price.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Name);
            writer.Write(Tier);
            writer.Write(CultivationElement);
            writer.Write(YinYangType);
            writer.Write(TypeRef);
            writer.Write(PropertyRef);
            writer.WriteArray(OriginRef);
            writer.Write(Desc1Ref);
            writer.Write(Desc2Ref);
            writer.WriteArray(ItemFlags);
            writer.Write(Price);
        }
#if UNITY_EDITOR
        public void Move()
        {
            var icon = Icon;
            var root = "Assets/Resources/";
            var path = Path.GetDirectoryName(root + IconPath);
            AssetOptimize.Move(icon.texture, path);
        }
#endif
    }
}