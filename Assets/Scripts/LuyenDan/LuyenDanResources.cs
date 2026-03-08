using System;
using System.Collections.Generic;
using System.Linq;
using LuyenDan.Inventory;
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
        [field: SerializeField] public List<HerbData> HerbDatas { get; private set; }
        public IInventoryItemData FindItem(ItemRef itemRef)
        {
            if (itemRef.Type == ItemType.Furnace)
            {
                return TempFurnaceDatas.FirstOrDefault(x => x.Id == itemRef.Id);
            }
            return HerbDatas.FirstOrDefault(x => x.Id == itemRef.Id);
        }
        public HerbData FindHerb(int id)
        {
            return HerbDatas.FirstOrDefault(x => x.Id == id);
        }
    }
}