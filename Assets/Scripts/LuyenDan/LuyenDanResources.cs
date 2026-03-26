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
        [field: SerializeField, ReadOnly] public List<LinhDan> LinhDans { get; private set; }
        [field: SerializeField, ReadOnly] public List<RecipeData> Recipes { get; private set; }
        public const string EmptyDetail = "Không";
        private static LuyenDanResources instance;
        public static LuyenDanResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<LuyenDanResources>("LuyenDan/LuyenDanResources");
                }
                return instance;
            }
        }
        public IItemData FindItem(ItemRef itemRef)
        {
            if (itemRef.Type == ItemType.Furnace)
            {
                return TempFurnaceDatas.FirstOrDefault(x => x.Id == itemRef.Id);
            }
            if (itemRef.Type == ItemType.Herb)
            {
                return HerbDatas.FirstOrDefault(x => x.Id == itemRef.Id);
            }
            return LinhDans.FirstOrDefault(x => x.Id == itemRef.Id);
        }
        public HerbData FindHerb(int id)
        {
            var herb = HerbDatas.FirstOrDefault(x => x.Id == id);
            return herb;
        }
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
