using System;
using System.Collections.Generic;
using System.Linq;
using LuyenDan.Inventory;
using Network;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class RecipeData
    {
        [Serializable]
        public class Herbs
        {
            [field: SerializeField] public int Id { get; private set; }
            [field: SerializeField] public int Count { get; private set; }
            [field: SerializeField] public int BaDuoc { get; private set; }

            public Herbs(int id, int count, int baDuoc)
            {
                Id = id;
                Count = count;
                BaDuoc = baDuoc;
            }

            public Herbs()
            {
            }

            public bool IsDefault()
            {
                return Id == 0;
            }
        }

        [Serializable]
        public class Recipe
        {
            [field: SerializeField] public int Id { get; private set; }
            [field: SerializeField] public Herbs ChuDuoc { get; private set; }
            [field: SerializeField] public Herbs PhuDuoc1 { get; private set; }
            [field: SerializeField] public Herbs PhuDuoc2 { get; private set; }
            [field: SerializeField] public Herbs DuocDan { get; private set; }

            public void Create(int id, int index, string[,] matrix)
            {
                Id = id;
                var colIndex = 2 + index * 6;
                ChuDuoc = GetHerbs(0, colIndex, matrix);
                PhuDuoc1 = GetHerbs(2, colIndex, matrix);
                PhuDuoc2 = GetHerbs(3, colIndex, matrix);
                DuocDan = GetHerbs(4, colIndex, matrix);
            }

            private Herbs GetHerbs(int rowIndex, int colIndex, string[,] matrix)
            {
                var countText = matrix[rowIndex, colIndex];
                int.TryParse(countText, out var count);
                if (count == 0)
                {
                    return null;
                }

                var name = matrix[rowIndex, colIndex + 1];
                var h = LuyenDanResources.Instance.HerbDatas.FirstOrDefault(x =>
                    x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (h == null)
                {
                    Debug.LogError($"There is no herbs with name '{name}'");
                }

                var id = h.Id;
                var baduoc = int.Parse(matrix[rowIndex, colIndex + 4]);
                return new Herbs(id, count, baduoc);
            }
        }

        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public int LinhDanId { get; set; }

        [ShowInInspector, ReadOnly, PreviewField(75)]
        public Sprite Icon
        {
            get
            {
                var linhDan = LinhDan;
                if (linhDan == null)
                {
                    return null;
                }

                return linhDan.Icon;
            }
        }

        [field: SerializeField] public List<Recipe> Recipes { get; private set; }

        public LinhDan LinhDan
        {
            get { return LuyenDanResources.Instance.LinhDans.FirstOrDefault(x => x.Id == LinhDanId); }
        }
    }
}