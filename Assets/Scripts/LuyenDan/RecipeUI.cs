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
        [SerializeField] private GameObject craftale;
        [SerializeField] private Transform listTf;
        [SerializeField] private GameObject unselected;
        [SerializeField] private GameObject selected;
        [SerializeField] private MiniRecipeUI prefab;
        private RecipeData data;
        private List<MiniRecipeUI> recipes = new List<MiniRecipeUI>();
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

        private void SetSize(bool active)
        {
            var size = thisTransform.sizeDelta;
            if (active)
            {
                size.y = 72 + 93 * data.Recipes.Count;
            }
            else
            {
                size.y = 72;
            }
            thisTransform.sizeDelta = size;
        }
        private void OnValueChanged(bool active)
        {
            SetSize(active);
            unselected.SetActive(!active);
            selected.SetActive(active);
        }
    }
}
