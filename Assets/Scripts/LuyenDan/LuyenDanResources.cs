using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Base;
using LongDH.LongDHModules;
using LuyenDan.Inventory;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenDan
{
    [CreateAssetMenu(fileName = "LuyenDanResources", menuName = "LuyenDan/Luyen Dan Resources")]
    public class LuyenDanResources : ScriptableObject, IMemorySerializable<LuyenDanResources>
    {
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
#if UNITY_EDITOR
        [Button]
        public void ShowSpriteSize()
        {
            var sprites = TempFurnaceDatas.Select(x => new[] { x.Icon, x.BigIcon }).SelectMany(x => x)
                .Concat(HerbDatas.Select(x => x.Icon))
                .Concat(LinhDans.Select(x => x.Icon))
                .ToHashSet();
            var size = AssetSize.CalculateSize(sprites);
            Debug.Log(size);
        }
#endif
        public IItemData FindItem(FurnaceData furnace)
        {
            Debug.Log(furnace.Id);
            return TempFurnaceDatas.FirstOrDefault(x => x.Id == furnace.Id);
        }

        public IItemData FindItem(ItemRef itemRef)
        {
            if (itemRef.Type == ItemType.Furnace)
            {
                return TempFurnaceDatas.FirstOrDefault(x => x.Id == itemRef.TempId);
            }

            if (itemRef.Type == ItemType.Herb)
            {
                return HerbDatas.FirstOrDefault(x => x.Id == itemRef.Id);
            }

            if (itemRef.Type == ItemType.LinhDan)
            {
                return LinhDans.FirstOrDefault(x => x.Id == itemRef.Id);
            }

            if (itemRef.Type == ItemType.Recipe)
            {
                return Recipes.FirstOrDefault(x => x.Id == itemRef.Id);
            }

            return null;
        }

        public HerbData FindHerb(int id)
        {
            var herb = HerbDatas.FirstOrDefault(x => x.Id == id);
            return herb;
        }

        public LinhDan FindLinhDan(int id)
        {
            var item = LinhDans.FirstOrDefault(x => x.Id == id);
            return item;
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

        public void Deserialize(IReader reader, ref LuyenDanResources value)
        {
            value.TempFurnaceDatas = reader.ReadArray<TempFurnaceData>().ToList();
            value.HerbDatas = reader.ReadArray<HerbData>().ToList();
            value.LinhDans = reader.ReadArray<LinhDan>().ToList();
            value.Recipes = reader.ReadArray<RecipeData>().ToList();
        }

        public int ByteSize()
        {
            return TempFurnaceDatas.ByteSize() + HerbDatas.ByteSize() + LinhDans.ByteSize() + Recipes.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(TempFurnaceDatas);
            writer.WriteArray(HerbDatas);
            writer.WriteArray(LinhDans);
            writer.WriteArray(Recipes);
        }

        [Button]
        public void Export()
        {
            var path = Path.Combine(Application.dataPath, "Exports", "LuyenDanResources.bin");
            var writer = new Writer(ByteSize());
            writer.Write(this);
            File.WriteAllBytes(path, writer.Buffer);
            Debug.Log(path);
        }

        [Button]
        public void FindHerbs(string name)
        {
            var item = HerbDatas.FirstOrDefault(x => x.Name.Contains(name, StringComparison.InvariantCultureIgnoreCase));
            if (item == null)
            {
                Debug.Log(name + " not found");
                return;
            }
            Debug.Log($"{item.Name} - Id: {item.Id}");
        }
    }
}