using System;
using LuyenDan.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class Alchermy : MonoBehaviour
    {
        [SerializeField] private GameObject thisObject;
        [SerializeField] private Image furnaceImage;
        [SerializeField] private Text durabilityText;
        [SerializeField] private Sprite defaultFurnace;
        [field: SerializeField] public SelectNum SelectNum { get; private set; }
        private FurnaceInventoryItemUI currentSelectFurnace;
        private AlchemyEntity currentSelected;
        [SerializeField] private Ingredient main1;
        [SerializeField] private Ingredient sub1;
        [SerializeField] private Ingredient sub2;
        [SerializeField] private Ingredient primer;
        [SerializeField] private AlchermyFurnace furnace;
        public void SelectDanLu(FurnaceInventoryItemUI furnace)
        {
            if (currentSelectFurnace != null)
            {
                currentSelectFurnace.ItemRef.OnDurabilityChanged -= SetDurability;
            }
            currentSelectFurnace = furnace;
            thisObject.SetActive(true);
            if (furnace != null)
            {
                furnaceImage.sprite = furnace.Data.BigIcon;
                furnace.ItemRef.OnDurabilityChanged += SetDurability;
                SetDurability(furnace.ItemRef.Durability);
            }
            else
            {
                furnaceImage.sprite = defaultFurnace;
                SetDurabilityText(0);
            }
            this.furnace.Select(furnace);
        }

        private void SetDurabilityText(int durability)
        {
            durabilityText.text = $"{durability}/100";
        }
        private void SetDurability(int durability)
        {
            var dur = Math.Clamp(durability,0, 100);
            SetDurabilityText(dur);
            if (dur <= 0)
            {
                SelectDanLu(null);
            }
        }
        public void SetCurrentSelected(AlchemyEntity alchemyEntity)
        {
            currentSelected = alchemyEntity;
        }
        public void Select(InventoryItemUI item)
        {
            if (currentSelected != null)
            {
                currentSelected.Select(item);
            }
        }
    }
}