
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Base;
using LuyenDan;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{

    [CreateAssetMenu]
    public class LuyenKhiResources : ScriptableObject
    {
        [field: SerializeField] public MaterialGradeConfig[] MaterialGradeConfigs { get; private set; }
        [field: SerializeField] public EquipmentIcon[] EquipmentIcons { get; private set; }
        [field: SerializeField] public EquipmentTierConfig[] EquipmentTiers { get; private set; }
        [field: SerializeField] public EquipmentGradeConfig[] EquipmentGrades { get; private set; }
        [field: SerializeField] public Material[] Materials { get; private set; }
        [field: SerializeField] public LuyenKhiRewardItem[] RewardItems { get; private set; }
        [field: SerializeField] public LianQiEffect[] LianQiEffects { get; private set; }
        [field: SerializeField] public EffectTypeNameConfig[] EffectTypeNameConfigs { get; private set; }
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
        
    }
}