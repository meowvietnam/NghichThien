using UnityEngine;

namespace LuyenKhi
{
    [CreateAssetMenu]
    public class LuyenKhiResources : ScriptableObject
    {
        [field: SerializeField] public EquipmentIcon[] EquipmentIcons { get; private set; }
        [field: SerializeField] public EquipmentTierConfig[] EquipmentTiers { get; private set; }
        [field: SerializeField] public EquipmentGradeConfig[] EquipmentGrades { get; private set; }
    }
}