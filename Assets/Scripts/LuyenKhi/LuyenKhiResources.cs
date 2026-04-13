using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;

namespace LuyenKhi
{
    [CreateAssetMenu]
    public class LuyenKhiResources : ScriptableObject
    {
        [field: SerializeField] public EquipmentIcon[] EquipmentIcons { get; private set; }
        [field: SerializeField] public EquipmentTierConfig[] EquipmentTiers { get; private set; }
        [field: SerializeField] public EquipmentGradeConfig[] EquipmentGrades { get; private set; }
        [field: SerializeField] public List<Material> Materials { get; private set; }
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
    }
}