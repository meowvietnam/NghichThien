using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class RecipeUI : MonoBehaviour
    {
        [SerializeField] private Text recipeNameText;
        [SerializeField] private Toggle toggle;
        [SerializeField] private RectTransform thisTransform;
        [SerializeField] private GameObject[] craftales;
        [SerializeField] private Transform listTf;
        [SerializeField] private GameObject unselected;
        [SerializeField] private GameObject selected;
        [SerializeField] private MiniRecipeUI prefab;
        private RecipeData data;
        private List<MiniRecipeUI> recipes = new();
        [SerializeField] private GameObject thisObject;
        public RecipeData Data => data;
        public void Init(RecipeData data, ToggleGroup group)
        {
            this.data = data;
            recipeNameText.text = data.Name;
            toggle.onValueChanged.AddListener(OnValueChanged);
            foreach (var recipeData in data.Recipes)
            {
                var instance = Instantiate(prefab, listTf);
                instance.Init(recipeData, group);
                recipes.Add(instance);
            }
            toggle.SetIsOnWithoutNotify(false);
            SetSize(false);
        }
        public void SetCraftable(bool active)
        {
            foreach (var craftale in craftales)
            {
                craftale.SetActive(active);
            }
        }
        
        private void SetSize(bool active)
        {
            var size = thisTransform.sizeDelta;
            if (active)
            {
                size.y = 82 + 93 * data.Recipes.Count;
            }
            else
            {
                size.y = 52;
            }
            thisTransform.sizeDelta = size;
        }

        public void SetActive(bool active)
        {
            thisObject.SetActive(active);
        }
        private void OnValueChanged(bool active)
        {
            SetSize(active);
            unselected.SetActive(!active);
            selected.SetActive(active);
        }
        public void Refresh()
        {
            var myCraftable = false;
            foreach (var recipe in recipes)
            {
                var canCraft = recipe.IsCraftable();
                if (canCraft)
                {
                    myCraftable = true;
                }
                recipe.UpdateCraftale(canCraft);
            }
            SetCraftable(myCraftable);
        }
    }
}
