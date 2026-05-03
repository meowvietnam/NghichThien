using System;
using System.Linq;
using Base;
using LuyenDan;
using LuyenKhi;
using Network;
using Network.Extensions;
using UnityEngine;
using Material = LuyenKhi.Material;

namespace CreateCharacter
{
    [Serializable]
    public class Equipment : IMemorySerializable<Equipment>, IEquatable<Equipment>, IDeepClone<Equipment>
    {
        [field: SerializeField] public ushort SelfId { get; set; }
        [field: SerializeField] public ushort Id { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Effect[] Effects { get; set; }
        public LuyenKhiRewardItem Item => LuyenKhiResources.Instance.RewardItems.FirstOrDefault(x => x.Id == Id);
        public ushort[] MaterialIds { get; set; }
        public Material[] Materials => LuyenKhiResources.Instance.Materials.Where(x => MaterialIds.Contains((ushort)x.Id)).ToArray();
        public void Deserialize(IReader reader, ref Equipment value)
        {
            value.SelfId = reader.Read<ushort>();
            value.Id = reader.Read<ushort>();
            value.Name = reader.Read();
            value.Effects = reader.ReadArray<Effect>();
            value.MaterialIds = reader.ReadArray<ushort>();
        }
        public ItemRef CreateRef()
        {
            var itemType = GetItemType();
            Debug.Log($"[ItemRef] Created: ID={SelfId}, Type={itemType}");
            return new ItemRef()
            {
                Id = SelfId,
                Type = itemType,
                Count = 1
            };
        }
        public ItemType GetItemType()
        {
            var equipmentType = GetEquipmentType();
            return equipmentType switch
            {
                EquipmentType.Weapon => ItemType.VuKhi,
                EquipmentType.Armor => ItemType.PhongCu,
                EquipmentType.Jewelry => ItemType.TrangSuc,
                _ => ItemType.VuKhi

            };
        }
        public int ByteSize()
        {
            return SelfId.ByteSize() + Id.ByteSize() + Name.ByteSize() + Effects.ByteSize() +  MaterialIds.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(SelfId);
            writer.Write(Id);
            writer.Write(Name);
            writer.WriteArray(Effects);
            writer.WriteArray(MaterialIds);
        }

        public EquipmentType GetEquipmentType()
        {
            var artifactType = Item.Type;
            switch (artifactType)
            {
                case ArtifactType.PhiKiem:
                case ArtifactType.BaoChung:
                case ArtifactType.ChiHoan:
                case ArtifactType.PhiCham:
                case ArtifactType.BaoHap:
                    return EquipmentType.Weapon;
                case ArtifactType.TruongBao:
                case ArtifactType.GiapTru:
                    return EquipmentType.Armor;
                default:
                    return EquipmentType.Jewelry;
            }
        }
        public EquipmentGrade GetGrade()
        {
            var materials = Materials;
            var res = LuyenKhiResources.Instance;
            var matConfigs = res.MaterialGradeConfigs;
            var grade = 0;
            foreach (var material in materials)
            {
                var matConfig = matConfigs.First(x => x.Tier == material.Tier);
                grade += matConfig.Grade;
            }
            var equipmentGradeConfig = res.EquipmentGrades.Last(x => x.MinGrade <= grade);
            return equipmentGradeConfig.Grade;
        }
        public bool Equals(Equipment? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return SelfId == other.SelfId && Id == other.Id;
        }

        public Equipment DeepClone()
        {
            return new Equipment
            {
                SelfId = SelfId,
                Id = Id,
                Name = Name,
                Effects = Effects,
            };
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SelfId, Id);
        }
    }
}