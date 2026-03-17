using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
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

        protected override void OnClick()
        {
            var instance = LuyenDanPanel.Instance;
            instance.Alchermy.SetCurrentSelected(this);
            var inventory = instance.HerbInventory;
            inventory.SetActive(true);
            var itemList = inventory.ItemListUI as HerbItemListUI;
            itemList.Filter(herbType);
        }
        public void SetCount(int count)
        {
            var itemRef = Item.ItemRef;
            itemRef.SetCount(itemRef.Count + Count - count);
            Count = count;
            countText.text = count > 1 ? count.ToString() : "";
        }
        public override void Select(InventoryItemUI item)
        {
            var count = LuyenDanPanel.Instance.Alchermy.SelectNum.GetValue();
            Select(item,count);
        }
        public void Select(InventoryItemUI item, int count)
        {
            if (Item != item)
            {
                if (Item != null)
                {
                    Item.ItemRef.SetCount(Item.ItemRef.Count + Count);
                }
                Item = item;
            }
            var data = item.GetData();
            SetCount(count);
            SetData(data);
        }
        public override void OnLuyenDanCompleted()
        {
            if (Item == null) return;
            var itemRef = Item.ItemRef;
            if (itemRef.Count <= 0)
            {
                Count = 0;
                Item = null;
                SetData(null);
            }
        }
    }
}