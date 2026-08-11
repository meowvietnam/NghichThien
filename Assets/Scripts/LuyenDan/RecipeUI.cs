using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class RecipeUI : BaseBehaviour
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

        [SerializeField] private Text chuduocDetail;
        [SerializeField] private Text phuduocDetail;
        public Button btnInfor;
        public RecipeData Data => data;
        public bool IsCraftable { get; private set; }
        public void Init(RecipeData data, ToggleGroup group)
        {
            this.data = data;
            var hasRecipe = data.IsInPlayerInventory();
            var owner = hasRecipe ? "Đã có" : "Chưa có";
            var color = hasRecipe
                ? new Color32(100, 255, 140, 255)
                : new Color32(255, 110, 100, 255);
            recipeNameText.text =
                $"{data.LinhDan.Name} (<color=#{ColorUtility.ToHtmlStringRGB(color)}>{owner}</color>)";
            toggle.onValueChanged.AddListener(OnValueChanged);
            foreach (var recipeData in data.Recipes)
            {
                var instance = Instantiate(prefab, listTf);
                instance.Init(recipeData, group);
                recipes.Add(instance);
            }
            toggle.SetIsOnWithoutNotify(false);
            SetSize(false);
            var rData = data.Recipes.First();
            if (rData != null)
            {
                var chuduoc = rData.ChuDuoc;
                var phuduoc = rData.PhuDuoc1;
                var resources = LuyenDanPanel.Instance.Resources;
                if (chuduoc != null)
                {
                    chuduocDetail.text = resources.GetHerbDetail(chuduoc.Id,HerbType.ChuDuoc);
                }

                if (phuduoc != null)
                {
                    phuduocDetail.text = resources.GetHerbDetail(phuduoc.Id,HerbType.ChuDuoc);
                }
            }
            SetEventButton(btnInfor, () =>
            {
                ToolTipItems.Instance.ShowInfoItem(Data, new ItemRef
                {
                    Id = (ushort)Data.Id,
                    Type = ItemType.Recipe,
                    Count = 1,
                });
            });
        }

        public void Destroy()
        {
            Destroy(thisObject);
        }
        public void SetCraftable(bool active)
        {
            IsCraftable = active;
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
