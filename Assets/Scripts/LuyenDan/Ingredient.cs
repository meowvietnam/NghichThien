using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace LuyenDan
{

    public class Ingredient : AlchemyEntity
    {

        [TabGroup("Has Item")]
        [SerializeField] private Text countText;
        [SerializeField] private Text propertyText;
        [SerializeField] private HerbType herbType;
        public int Count { get; private set; }
        public void SetCount(int newCount)
        {
            int delta = Count - newCount;
            Item.ItemRef.SetCount(Item.ItemRef.Count + delta);
            Count = newCount;
            countText.text = newCount > 1 ? newCount.ToString() : string.Empty;
        }

        public override void Select(InventoryItemUI item)
        {
            int count = LuyenDanPanel.Instance.Alchermy.SelectNum.GetValue();
            Select(item, count);
        }
        public void Select(InventoryItemUI item, int count)
        {
            if (Item != item)
            {
                ReturnCurrentItemToInventory();
                Count = 0;
                Item = item;
                if (item == null)
                {
                    ClearSlot();
                    return;
                }
            }
            else if (item == null)
            {
                return;
            }
            SetCount(count);
            SetData(item.GetData());
            UpdatePropertyText();
        }

        private void UpdatePropertyText()
        {
            if (propertyText == null)
            {
                return;
            }

            if (Item == null)
            {
                propertyText.text = LuyenDanResources.EmptyDetail;
            }
            else
            {
                propertyText.text = LuyenDanPanel.Instance.Resources.GetHerbDetail(Item.ItemRef.Id,herbType);
            }
        }
        public override void OnLuyenDanCompleted()
        {
            if (Item == null) return;

            bool inventoryEmpty = Item.ItemRef.Count <= 0;
            if (inventoryEmpty)
            {
                ClearSlot();
            }
        }

        protected override void OnClick()
        {
            var panel = LuyenDanPanel.Instance;
            panel.Alchermy.SetCurrentSelected(this);

            var inventory = panel.HerbInventory;
            inventory.SetActive(true);

            var herbList = inventory.ItemListUI as HerbItemListUI;
            herbList.Filter(herbType);
        }
        private void ReturnCurrentItemToInventory()
        {
            if (Item == null) return;
            Item.ItemRef.SetCount(Item.ItemRef.Count + Count);
        }
        private void ClearSlot()
        {
            Count = 0;
            Item  = null;
            SetData(null);
            UpdatePropertyText();
        }
    }
}