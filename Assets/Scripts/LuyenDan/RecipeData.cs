using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenDan
{
    [System.Serializable]
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
        }
        [Serializable]
        public class Recipe
        {
            [field: SerializeField] public int Id { get; private set; }
            [field: SerializeField] public Herbs ChuDuoc { get; private set; }
            [field: SerializeField] public Herbs PhuDuoc1 { get; private set; }
            [field: SerializeField] public Herbs PhuDuoc2 { get; private set; }
            [field: SerializeField] public Herbs DuocDan { get; private set; }
            public void Create(int id, int index ,string[,] matrix)
            {
                Id = id;
                var colIndex = 2 + index * 6;
                ChuDuoc = GetHerbs(0,colIndex, matrix);
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
                var name = matrix[rowIndex, colIndex+1];
                var h = LuyenDanPanel.Instance.Resources.HerbDatas.FirstOrDefault(x => x.Name.Equals(name,StringComparison.InvariantCultureIgnoreCase));
                if (h == null)
                {
                    Debug.LogError($"There is no herbs with name '{name}'");
                }
                var id = h.Id;
                var baduoc = int.Parse(matrix[rowIndex, colIndex+4]);
                return new Herbs(id, count, baduoc);
            }
        }
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Type { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public int DanDoc { get; private set; }
        [field: SerializeField] public int NhinThuoc { get; private set; }
        [field: SerializeField] public int PriceV104 { get; private set; }
        [field: SerializeField] public int PriceV130 { get; private set; }
        [field: SerializeField] public int Price { get; private set; }
        [field: SerializeField] public int Profit { get; private set; }
        [field: SerializeField] public float Income { get; private set; }
        [field: SerializeField] public int ProfitPerDay { get; private set; }
        [field: SerializeField] public int ProfitDanThanh { get; private set; }
        [field: SerializeField] public int ProfitDanThanhPerDay { get; private set; }
        [field: SerializeField] public float IncomeDanThanh { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public List<Recipe> Recipes { get; private set; }
        public void Create(int id, string data)
        {
            Id = id;
            var p = data.Split('\t');
            Tier = (Tier)(int.Parse(p[0]) - 1);
            Name = p[1].Trim();
            Type = p[2].Trim();
            Description = p[3].Trim();
            int.TryParse(p[4], out var danDoc);
            DanDoc = danDoc;
            int.TryParse(p[5], out var nhinThuoc);
            NhinThuoc = nhinThuoc;
            int.TryParse(p[6], out var priceV104);
            PriceV104 = priceV104;
            int.TryParse(p[7], out var priceV130);
            PriceV130 = priceV130;
            int.TryParse(p[8], out var price);
            Price = price;
            int.TryParse(p[9], out var profit);
            Profit = profit;
            float.TryParse(
                p[10].Replace(',', '.').Replace("%", ""),
                out var income
            );
            Income = income;
            int.TryParse(p[11], out var profitPerDay);
            ProfitPerDay = profitPerDay;
            int.TryParse(p[12], out var profitDanThanh);
            ProfitDanThanh = profitDanThanh;
            int.TryParse(p[13], out var profitDanThanhPerDay);
            ProfitDanThanhPerDay = profitDanThanhPerDay;
            float.TryParse(
                p[14].Replace(',', '.').Replace("%", ""),
                out var incomeDanThanh
            );
            IncomeDanThanh = incomeDanThanh;
        }

        public void ImportReipes(string[] data)
        {
            var s = data.Select(x => x.Split('\t')).ToList();
            var matrix = new string[5, 20];
            for (int i = 0; i < s.Count; i++)
            {
                var line = s[i];
                for (int j = 0; j < line.Length; j++)
                {
                    matrix[i, j] = line[j].Trim();
                }
            }
            int totalRecipe = 0;
            for (int i = 2; i < matrix.GetLength(1); i+=6)
            {
                var item = matrix[0,i];
                if (string.IsNullOrEmpty(item) || item == "0")
                {
                    continue;
                }
                totalRecipe++;
            }
            Recipes = new List<Recipe>(totalRecipe);
            for (int i = 0; i < totalRecipe; i++)
            {
                var recipe = new Recipe();
                recipe.Create(Id + i + 1,i,matrix);
                Recipes.Add(recipe);
            }
        }

        public void SetIcon(Sprite icon)
        {
            Icon = icon;
        }
    }
}