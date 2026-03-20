using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LuyenDan.Inventory
{
    public class HerbInventory : Inventory
    {
        public List<ItemSnapshot> ItemsSnapshot { get; private set; }
        public void Init(IEnumerable<ItemRef> data)
        {
            ItemListUI.Init(data);
            ItemsSnapshot = data.Select(x => new ItemSnapshot(x.Id,x.Count)).ToList();
        }

        private List<HerbInventoryItemUI> HerbInventoryItemUIs => (ItemListUI as HerbItemListUI).Items;
        public void Refresh()
        {
            var items = HerbInventoryItemUIs;
            ItemsSnapshot = items
                .Select(x =>
                {
                    var itemRef = x.ItemRef;
                    return new ItemSnapshot(itemRef.Id,itemRef.Count);
                }).ToList();
        }

        public HerbInventoryItemUI GetItem(int id)
        {
            return HerbInventoryItemUIs.FirstOrDefault(x => x.ItemRef.Id == id);
        }
    }
}