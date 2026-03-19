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
        private FurnaceInventoryItemUI selectedFurnace;
        private AlchemyEntity pendingSelectionTarget;

        public void SetCurrentSelected(AlchemyEntity target)
        {
            pendingSelectionTarget = target;
        }

        public void Select(InventoryItemUI item)
        {
            pendingSelectionTarget?.Select(item);
            refineButton.UpdateCanRefine();
        }

        public void SelectDanLu(FurnaceInventoryItemUI furnace)
        {
            selectedFurnace = furnace;
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
            if (selectedFurnace != null)
                SetDurabilityText(selectedFurnace.ItemRef.Durability);

            furnaceSlot.OnLuyenDanCompleted();
            main1.OnLuyenDanCompleted();
            sub1.OnLuyenDanCompleted();
            sub2.OnLuyenDanCompleted();
            primer.OnLuyenDanCompleted();

            if (selectedFurnace != null && selectedFurnace.ItemRef.Durability <= 0)
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
    }
}