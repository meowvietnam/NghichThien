using System;
using System.Collections.Generic;
using System.Linq;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEditor;
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
        #region import

        // [OnValueChanged("OnValueChanged")]
        // [SerializeField] private int startId;
        // [OnValueChanged("OnValueChanged")]
        // [TextArea]
        // [SerializeField] private string text;
        // private void OnValueChanged()
        // {
        //     if (string.IsNullOrWhiteSpace(text)) return;
        //     var lines = text.Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        //     HerbDatas = new List<HerbData>(lines.Length);
        //     for (int i = 0;i< lines.Length;i++)
        //     {
        //         try
        //         {
        //             var line = lines[i];
        //             var parts = line.Split('\t')
        //                 .ToList();
        //             parts.RemoveAt(5);
        //             var id = startId + i;
        //             var tier = (Tier)(int.Parse(parts[0]) - 1);
        //             var name = parts[1];
        //             var chuduoc = parts[2];
        //             var phuduoc = parts[3];
        //             var duocdan = parts[4];
        //             var locations = parts[5];
        //             var herbData = new HerbData(id,tier, name, chuduoc, phuduoc,duocdan, locations);
        //             HerbDatas.Add(herbData);
        //         }
        //         catch (Exception ex)
        //         {
        //             Debug.LogException(ex);
        //         }
        //     }
        //     EditorUtility.SetDirty(this);
        // }

        #endregion
        
    }
}
