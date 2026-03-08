using System;
using System.Collections.Generic;
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
        }
        [Serializable]
        public class Recipe
        {
            [field: SerializeField] public int Id { get; private set; }
            [field: SerializeField] public Herbs ChuDuoc { get; private set; }
            [field: SerializeField] public Herbs PhuDuoc1 { get; private set; }
            [field: SerializeField] public Herbs PhuDuoc2 { get; private set; }
            [field: SerializeField] public Herbs ThuocDan { get; private set; }
        }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public List<Recipe> Recipes { get; private set; }
    }
}