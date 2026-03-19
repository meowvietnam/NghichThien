using LuyenDan.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class AlchermyFurnace : AlchemyEntity
    {
        [SerializeField] private Text durabilityText;
        public override void Select(InventoryItemUI item)
        {
            if (item != null && item is not FurnaceInventoryItemUI)
            {
                Debug.LogWarning(
                    $"[AlchermyFurnace] Select nhận sai type: {item.GetType().Name}. Bỏ qua.");
                return;
            }
            LuyenDanPanel.Instance.Alchermy.SelectDanLu(item as FurnaceInventoryItemUI);
        }
        public void Select(FurnaceInventoryItemUI item)
        {
            Item = item;

            if (item != null)
            {
                SetData(item.GetData());
                RefreshDurability(item.ItemRef.Durability);
            }
            else
            {
                ClearSlot();
            }
        }

        public override void OnLuyenDanCompleted()
        {
            if (Item == null) return;
            RefreshDurability(Item.ItemRef.Durability);
        }


        protected override void OnClick()
        {
            var panel = LuyenDanPanel.Instance;
            panel.Alchermy.SetCurrentSelected(this);
            panel.FurnaceInventory.SetActive(true);
            panel.FurnaceInventory.ItemListUI.NoFilter();
        }
        private void RefreshDurability(int value)
        {
            if (value <= 0)
            {
                ClearSlot();
                return;
            }

            durabilityText.text = $"{value}/100";
        }
        private void ClearSlot()
        {
            Item = null;
            SetData(null);
            durabilityText.text = string.Empty;
        }
    }
}