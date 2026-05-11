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
        public EquipmentGradeConfig GradeConfig => LuyenKhiResources.Instance.EquipmentGrades.FirstOrDefault(x=>x.Grade == GetGrade());

        public Material[] GetMaterialsWithEffectType(EffectType effectType)
        {
            // 1. Lấy ArtifactType từ Item
            ArtifactType artifactType = Item.Type;

            // 2. Tìm Config chứa cặp Element/YinYang tương ứng
            var config = LuyenKhiResources.Instance.LianQiEffects.FirstOrDefault(c =>
                c.Effects.Any(e => e.EffectType == effectType && e.ArtifactType == artifactType)
            );

            // Nếu không tìm thấy cấu trúc phù hợp, trả về mảng rỗng thay vì null để tránh lỗi foreach bên ngoài
            if (config == null) return System.Array.Empty<Material>();

            // 3. Lọc TẤT CẢ nguyên liệu khớp với Hệ và Âm Dương của Effect đó
            return Materials.Where(m =>
                m.CultivationElement == config.CultivationElement &&
                m.YinYangType == config.YinYangType
            ).ToArray();
        }
        public (int min, int max) GetMinMaxValue(EffectType effectType)
        {
            ArtifactType artifactType = Item.Type;
            var allConfigs = LuyenKhiResources.Instance.LianQiEffects;

            // 1. Tìm TẤT CẢ các nhóm (Element/YinYang) có chứa EffectType này cho ArtifactType này
            var validConfigs = allConfigs.Where(c =>
                c.Effects.Any(e => e.EffectType == effectType && e.ArtifactType == artifactType)
            ).ToList();

            if (validConfigs.Count == 0) return (0, 0);

            // Lấy hệ số Value (Giả định cùng EffectType thì Value giống nhau, lấy cái đầu tiên)
            var effectData = validConfigs[0].Effects.First(e => e.EffectType == effectType && e.ArtifactType == artifactType);
            if (effectData.Value == 0) return (0, 0);

            // 2. Tính TỔNG Grade của nguyên liệu thuộc TẤT CẢ các hệ hợp lệ đã tìm thấy
            int sumGrade = 0;
            foreach (var config in validConfigs)
            {
                sumGrade += Materials
                    .Where(m => m.CultivationElement == config.CultivationElement
                             && m.YinYangType == config.YinYangType)
                    .Sum(m => LuyenKhiResources.Instance.MaterialGradeConfigs
                        .FirstOrDefault(x => x.Tier == m.Tier)?.Grade ?? 0);
            }

            // 3. Tính toán theo công thức cộng dồn giống Recalculate
            int maxValue = Mathf.RoundToInt(1.0f * sumGrade / effectData.Value);
            int minValue = Mathf.RoundToInt(0.5f * maxValue);

            Debug.Log($"Tổng Grade gom được: {sumGrade} | Min: {minValue} Max: {maxValue}");
            return (minValue, maxValue);
        }
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