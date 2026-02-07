using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenDan
{
    [CreateAssetMenu(fileName = "LuyenDanResources", menuName = "LuyenDan/Luyen Dan Resources")]
    public class LuyenDanResources : ScriptableObject
    {
        [TabGroup("Inventory")]
        [field: SerializeField] public List<Quality> Qualities { get; private set; }
        [field: SerializeField] public List<TempFurnaceData> TempFurnaceDatas { get; private set; }
    }
}