
using System;
using System.Linq;
using Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class C
    {
        [field: SerializeField]
        public string Key { get; set; }
        [field: SerializeField]
        public CultivationElement Value { get; set; }
    }

    [Serializable]
    public class T
    {
        [field: SerializeField]
        public string Key { get; set; }
        [field: SerializeField]
        public YinYangType Value { get; set; }
    }
    [CreateAssetMenu]
    public class LuyenKhiResources : ScriptableObject
    {
        [field: SerializeField] public EquipmentIcon[] EquipmentIcons { get; private set; }
        [field: SerializeField] public EquipmentTierConfig[] EquipmentTiers { get; private set; }
        [field: SerializeField] public EquipmentGradeConfig[] EquipmentGrades { get; private set; }
        [field: SerializeField] public Material[] Materials { get; private set; }
        [field: SerializeField] public LuyenKhiRewardItem[] RewardItems { get; private set; }
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

        [Button]
        public void Load()
        {
            foreach (var item in RewardItems)
            {
                item.Load();
            }
        }
    }
}