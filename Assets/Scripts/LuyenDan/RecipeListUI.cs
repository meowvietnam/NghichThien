using System.Collections.Generic;
using System.Linq;
using Base;
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
        [SerializeField] private Text filterText;
        private Dictionary<Tier, string> map = new()
        {
            {Tier.NhatPham, "Nhất phẩm"},
            {Tier.NhiPham, "Nhị phẩm"},
            {Tier.TamPham, "Tam phẩm"},
            {Tier.TuPham, "Tứ phẩm"},
            {Tier.NguPham, "Ngũ phẩm"},
            {Tier.LucPham, "Lục phẩm"}
        };
        private const string allFilter = "Tất cả phẩm giai";
        public void Init(List<RecipeData> datas)
        {
            foreach (var recipeUI in recipes)
            {
                recipeUI.Destroy();
            }
            recipes.Clear();
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
            filterText.text = allFilter;
            foreach (var recipeUI in recipes)
            {
                recipeUI.SetActive(true);
            }
        }
        public void Filter(Tier tier)
        {
            filterText.text = map[tier];
            foreach (var recipeUI in recipes)
            {
                var active = recipeUI.Data.LinhDan.Tier == tier;
                recipeUI.SetActive(active);
            }
        }
        public (RecipeData data, RecipeData.Recipe recipe) FindBestRecipe(Ingredient main1, Ingredient sub1, Ingredient sub2, Ingredient primer)
        {
            foreach (var recipeUI in recipes)
            {
                var data = recipeUI.Data;
                if (data == null || data.Recipes == null) continue;

                foreach (var recipe in data.Recipes)
                {
                    if (recipe == null) continue;

                    if (!IsMatch(recipe.ChuDuoc, main1)) continue;
                    if (!IsMatch(recipe.DuocDan, primer)) continue;

                    bool matchSub1 = IsMatch(recipe.PhuDuoc1, sub1) && IsMatch(recipe.PhuDuoc2, sub2);
                    bool matchSub2 = IsMatch(recipe.PhuDuoc1, sub2) && IsMatch(recipe.PhuDuoc2, sub1);

                    if (matchSub1 || matchSub2)
                    {
                        return (data, recipe);
                    }
                }
            }
            return (null, null);
        }

        private bool IsMatch(RecipeData.Herbs herb, Ingredient ingredient)
        {
            bool herbEmpty = herb == null || herb.IsDefault();
            bool ingEmpty = ingredient == null || ingredient.IsEmpty || ingredient.Count == 0;

            if (herbEmpty && ingEmpty) return true;
            if (herbEmpty || ingEmpty) return false;

            return herb.Id == ingredient.Item.ItemRef.Id && herb.Count == ingredient.Count;
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