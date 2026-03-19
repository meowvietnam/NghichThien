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
        [field: SerializeField, ReadOnly] public List<Quality> Qualities { get; private set; }
        [field: SerializeField, ReadOnly] public List<TempFurnaceData> TempFurnaceDatas { get; private set; }
        [field: SerializeField, ReadOnly] public List<HerbData> HerbDatas { get; private set; }
        [field: SerializeField, ReadOnly] public List<RecipeData> Recipes { get; private set; }
        public const string EmptyDetail = "Không";
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
            var herb = HerbDatas.FirstOrDefault(x => x.Id == id);
            return herb;
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
        //
        // private void OnValueChanged()
        // {
        //     if (string.IsNullOrWhiteSpace(text))
        //     {
        //         return;
        //     }
        //     var parts = text.Split('\n');
        //     for (var i = 0; i < parts.Length; i++)
        //     {
        //         var id = startId + i*10;
        //         var part = parts[i];
        //         var recipe = Recipes[i];
        //         recipe.Create(id,part);
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
        #region import4
        // [TextArea]
        // [OnValueChanged("OnChanged")]
        // [SerializeField] private string text;
        // [SerializeField] public List<Sprite> sprites;
        // private void OnChanged()
        // {
        //     try
        //     {
        //         var list = JsonConvert.DeserializeObject<List<T>>(text);
        //         foreach (var recipeData in Recipes)
        //         {
        //             var r = list.FirstOrDefault(x => x.Name.Equals(recipeData.Name, StringComparison.InvariantCultureIgnoreCase));
        //             if (r == null)
        //             {
        //                 Debug.LogError($"Recipe {recipeData.Id} {recipeData.Name} not found");
        //                 recipeData.SetIcon(null);
        //                 continue;
        //             }
        //             var iconName = r.IconName;
        //             var icon = sprites.FirstOrDefault(x => x.name.Equals(iconName, StringComparison.InvariantCultureIgnoreCase));
        //             if (icon == null)
        //             {
        //                 Debug.LogError($"Sprite {iconName} - {recipeData.Id} not found");
        //             }
        //             recipeData.SetIcon(icon);
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError(e);
        //     }
        //     EditorUtility.SetDirty(this);
        // }
        // class T
        // {
        //     public int Id { get; set; }
        //     public string IconName { get; set; }
        //     public string Name { get; set; }
        // }
        #endregion

        public string GetHerbDetail(int id, HerbType herbType)
        {
            var herb = HerbDatas.FirstOrDefault(x => x.Id == id);
            if (herb == null)
            {
                return EmptyDetail;
            }
            switch (herbType)
            {
                case HerbType.ChuDuoc:
                    return herb.ChuDuoc;
                case HerbType.PhuDuoc:
                    return herb.PhuDuoc;
                case HerbType.DuocDan:
                    return herb.ThuocDan;
            }

            return EmptyDetail;
        }
        public RecipeData FindRecipe(int i)
        {
            return Recipes.FirstOrDefault(x => x.Id == i);
        }
    }
}
