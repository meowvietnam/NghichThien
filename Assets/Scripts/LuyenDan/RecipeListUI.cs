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