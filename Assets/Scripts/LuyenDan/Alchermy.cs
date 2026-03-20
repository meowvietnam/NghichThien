using System.Linq;
using LuyenDan.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class Alchermy : MonoBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Image furnaceImage;
        [SerializeField] private Text durabilityText;
        [SerializeField] private Sprite defaultFurnaceSprite;

        [SerializeField] private Ingredient main1;
        [SerializeField] private Ingredient sub1;
        [SerializeField] private Ingredient sub2;
        [SerializeField] private Ingredient primer;
        [SerializeField] private AlchermyFurnace furnaceSlot;
        [field: SerializeField] public SelectNum SelectNum { get; private set; }
        [SerializeField] private RefineButton refineButton;
        public FurnaceInventoryItemUI SelectedFurnace { get; private set; }
        private AlchemyEntity pendingSelectionTarget;
        private InventoryItemUI pendingSelectionItem;
        public void SetCurrentSelected(AlchemyEntity target)
        {
            pendingSelectionTarget = target;
        }
        //Event function
        public void ConfirmSelected()
        {
            pendingSelectionTarget?.Select(pendingSelectionItem);
            refineButton.UpdateCanRefine();
        }
        public void Select(InventoryItemUI item)
        {
            pendingSelectionItem = item;
            var count = item.ItemRef.Count;
            if (pendingSelectionTarget is Ingredient ingredient)
            {
                if (ingredient.Item == item)
                {
                    count += ingredient.Count;
                    SelectNum.SetValue(ingredient.Count);
                }
                else
                {
                    SelectNum.SetValue(1);
                }
            }
            SelectNum.SetMax(count);
            var herb = LuyenDanPanel.Instance.Resources.FindHerb(item.ItemRef.Id);
            SelectNum.SetText(herb.Name);
            SelectNum.SetActive(true);
        }

        public void SelectDanLu(FurnaceInventoryItemUI furnace)
        {
            SelectedFurnace = furnace;
            selfObject.SetActive(true);
            if (furnace != null)
            {
                furnaceImage.sprite = furnace.Data.BigIcon;
                SetDurabilityText(furnace.ItemRef.Durability);
            }
            else
            {
                furnaceImage.sprite = defaultFurnaceSprite;
                SetDurabilityText(0);
            }
            furnaceSlot.Select(furnace);
            refineButton.UpdateCanRefine();
        }

        public void OnLuyenDanCompleted()
        {
            if (SelectedFurnace != null)
                SetDurabilityText(SelectedFurnace.ItemRef.Durability);

            furnaceSlot.OnLuyenDanCompleted();
            main1.OnLuyenDanCompleted();
            sub1.OnLuyenDanCompleted();
            sub2.OnLuyenDanCompleted();
            primer.OnLuyenDanCompleted();

            if (SelectedFurnace != null && SelectedFurnace.ItemRef.Durability <= 0)
                SelectDanLu(null);
            refineButton.UpdateCanRefine();
        }

        private void SetDurabilityText(int durability)
        {
            durabilityText.text = $"{durability}/100";
        }

        public void SelectReceipe(RecipeData.Recipe data)
        {
            var chuduoc = data.ChuDuoc;
            var phuduoc1 = data.PhuDuoc1;
            var phuduoc2 = data.PhuDuoc2;
            var duocdan = data.DuocDan;
            Select(main1, chuduoc);
            Select(sub1, phuduoc1);
            Select(sub2, phuduoc2);
            Select(primer, duocdan);
            refineButton.UpdateCanRefine();
        }

        private void Select(Ingredient ingredient, RecipeData.Herbs herb)
        {
            if (herb == null)
            {
                ingredient.Select(null);
            }
            else
            {
                var inventory = LuyenDanPanel.Instance.HerbInventory;
                ingredient.Select(inventory.GetItem(herb.Id), herb.Count);
            }
        }
        public void ClearAllSlot()
        {
            main1.Select(null);
            sub1.Select(null);
            sub2.Select(null);
            primer.Select(null);
            refineButton.UpdateCanRefine();
        }

        public bool CanRefine()
        {
            if (furnaceSlot.IsEmpty)
            {
                return false;
            }
            Ingredient[] v = {main1, sub1, sub2, primer};
            return v.Any(x => !x.IsEmpty);
        }

        public void Refine()
        {
            var (data,recipe) = LuyenDanPanel.Instance.RecipeListUI.FindBestRecipe(main1, sub1, sub2, primer);
            if (data == null)
            {
                Debug.Log("No recipe found");
                return;
            }
            Debug.Log($"{data.LinhDan.Name}: {recipe != null}");
        }
    }
}