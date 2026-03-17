using System;
using LuyenDan.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class AlchermyFurnace : AlchemyEntity
    {
        [SerializeField] private Text durabilityText;
        protected override void OnClick()
        {
            var instance = LuyenDanPanel.Instance;
            instance.Alchermy.SetCurrentSelected(this);
            instance.FurnaceInventory.SetActive(true);
        }
        public override void Select(InventoryItemUI item)
        {
            var instance = LuyenDanPanel.Instance;
            instance.Alchermy.SelectDanLu(item as FurnaceInventoryItemUI);
        }

        public void Select(FurnaceInventoryItemUI item)
        {
            Item = item;
            if (item != null)
            {
                var data = item.GetData();
                SetData(data);
                SetDurability(item.ItemRef.Durability);
            }
            else
            {
                SetDurability(0);
            }
        }

        private void SetDurability(int value)
        {
            if (value <= 0)
            {
                Item = null;
                SetData(null);
            }
            durabilityText.text = $"{value}/100";
        }
        public override void OnLuyenDanCompleted()
        {
            if (Item == null) return;
            var itemRef = Item.ItemRef;
            var durability = itemRef.Durability;
            SetDurability(durability);
        }
    }
}