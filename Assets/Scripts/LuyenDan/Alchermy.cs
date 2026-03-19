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

        private FurnaceInventoryItemUI selectedFurnace;
        private AlchemyEntity pendingSelectionTarget;

        public void SetCurrentSelected(AlchemyEntity target)
        {
            pendingSelectionTarget = target;
        }

        public void Select(InventoryItemUI item)
        {
            pendingSelectionTarget?.Select(item);
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
        }

        private void SetDurabilityText(int durability)
        {
            durabilityText.text = $"{durability}/100";
        }
    }
}