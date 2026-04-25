using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Base;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [CreateAssetMenu]
    public class LuyenKhiResources : ScriptableObject, IMemorySerializable<LuyenKhiResources>
    {
        [field: SerializeField] public MaterialGradeConfig[] MaterialGradeConfigs { get; private set; }
        [field: SerializeField] public EquipmentIcon[] EquipmentIcons { get; private set; }
        [field: SerializeField] public EquipmentTierConfig[] EquipmentTiers { get; private set; }
        [field: SerializeField] public EquipmentGradeConfig[] EquipmentGrades { get; private set; }
        [field: SerializeField] public Material[] Materials { get; private set; }
        [field: SerializeField] public LuyenKhiRewardItem[] RewardItems { get; private set; }
        [field: SerializeField] public LianQiEffect[] LianQiEffects { get; private set; }
        [field: SerializeField] public EffectTypeConfig[] EffectTypeConfigs { get; private set; }
        [field: SerializeField] public StringDict Strings { get; private set; } = new();
        private static LuyenKhiResources instance;

        public static LuyenKhiResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<LuyenKhiResources>("LuyenKhi/LuyenKhiResources");
                }

                return instance;
            }
        }

        public IItemData FindItem(ItemRef itemRef)
        {
            var type = itemRef.Type;
            if (type == ItemType.Material)
            {
                return Materials.First(x => x.Id == itemRef.Id);
            }

            return null;
        }


        public void Deserialize(IReader reader, ref LuyenKhiResources value)
        {
            value.MaterialGradeConfigs = reader.ReadArray<MaterialGradeConfig>();
            value.EquipmentTiers = reader.ReadArray<EquipmentTierConfig>();
            value.EquipmentGrades = reader.ReadArray<EquipmentGradeConfig>();
            value.Materials = reader.ReadArray<Material>();
            value.LianQiEffects = reader.ReadArray<LianQiEffect>();
            value.EffectTypeConfigs = reader.ReadArray<EffectTypeConfig>();
            value.Strings = reader.Read<StringDict>();
        }
        public int ByteSize()
        {
            return MaterialGradeConfigs.ByteSize() + EquipmentTiers.ByteSize()
                                                   + EquipmentGrades.ByteSize() + Materials.ByteSize()
                                                   + LianQiEffects.ByteSize()
                                                   + EffectTypeConfigs.ByteSize() + Strings.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(MaterialGradeConfigs);
            writer.WriteArray(EquipmentTiers);
            writer.WriteArray(EquipmentGrades);
            writer.WriteArray(Materials);
            writer.WriteArray(LianQiEffects);
            writer.WriteArray(EffectTypeConfigs);
            writer.Write(Strings);
        }
    }
}