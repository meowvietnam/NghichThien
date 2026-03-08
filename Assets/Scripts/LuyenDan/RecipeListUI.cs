using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class RecipeListUI : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private RecipeUI prefab;
        private List<RecipeUI> recipes = new List<RecipeUI>();
        public void Init(List<RecipeData> datas) 
        {
            foreach (var recipe in datas)
            {
                var instance = Instantiate(prefab, content);
                instance.Init(recipe,toggleGroup);
                recipes.Add(instance);
            }
        }
    }
}