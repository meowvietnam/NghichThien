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
        [field: SerializeField] public List<RecipeData> Recipes { get; private set; }
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
        #region import1

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
        //     foreach (var herbData in HerbDatas)
        //     {
        //         herbData.Modify();
        //     }
        //     EditorUtility.SetDirty(this);
        // }

        #endregion
        #region import2
        // [OnValueChanged("OnValueChanged")]
        // [SerializeField] private int startId;
        // [OnValueChanged("OnValueChanged")]
        // [TextArea]
        // [SerializeField] private string text;
        
        // private void OnValueChanged()
        // {
        //     if (string.IsNullOrWhiteSpace(text))
        //     {
        //         return;
        //     }
        //     var parts = text.Split('\n');
        //     Recipes = new List<RecipeData>(parts.Length);
        //     for (var i = 0; i < parts.Length; i++)
        //     {
        //         var id = startId + i*10;
        //         var part = parts[i];
        //         var recipe = new RecipeData();
        //         recipe.Create(id,part);
        //         Recipes.Add(recipe);
        //     }
        //     foreach (var herbData in HerbDatas)
        //     {
        //         herbData.Modify();
        //     }
        //     EditorUtility.SetDirty(this);
        // }
        #endregion
        #region import3

        // [TextArea]
        // [OnValueChanged("OnChanged")]
        // [SerializeField] private string text;
        // public void OnChanged()
        // {
        //     if (string.IsNullOrEmpty(text)) return;
        //
        //     var lines = text.Split("\n");
        //     var datas = lines
        //         .Select((line, index) => new { Line = line, Index = index })
        //         .GroupBy(x => x.Index / 5)
        //         .Select(x => x
        //             .ToList()
        //             .Select(y => y.Line)
        //             .ToArray())
        //         .ToList();
        //     foreach (var data in datas)
        //     {
        //         var name = data[0].Split('\t')[0].Trim();
        //         var recipeData = Recipes.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        //         if (recipeData == null)
        //         {
        //             Debug.LogError($"Recipe {name} not found");
        //         }
        //         recipeData.ImportReipes(data);
        //     }
        //     EditorUtility.SetDirty(this);
        // }

        #endregion
    }
}
