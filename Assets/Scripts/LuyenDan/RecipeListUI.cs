using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private GameObject tierSelection;
        public void Init(List<RecipeData> datas) 
        {
            foreach (var recipe in datas)
            {
                var instance = Instantiate(prefab, content);
                instance.Init(recipe,toggleGroup);
                recipes.Add(instance);
            }
            Refresh();
        }
        public void ShowAll()
        {
           
        }
        public void Filter(Tier tier)
        {
            
        }
        public void OpenTierSelection()
        {
            tierSelection.SetActive(true);
        }

        public void CloseTierSelection()
        {
            tierSelection.SetActive(false);
        }
        public void Refresh()
        {
            foreach (var recipeUI in recipes)
            {
                recipeUI.Refresh();
            }
        }
    }
}