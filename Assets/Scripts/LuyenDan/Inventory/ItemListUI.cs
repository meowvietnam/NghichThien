using System.Collections.Generic;
using Base;
using UnityEngine;

namespace LuyenDan.Inventory
{
    public abstract class ItemListUI : MonoBehaviour
    {
        public abstract void Init(IEnumerable<ItemRef> datas);

        public abstract void Filter(Tier tier);
        public abstract void NoFilter();
    }
    public abstract class ItemListUI<T> : ItemListUI where T : InventoryItemUI
    {
        public List<T> Items { get; private set;} = new List<T>();
        [SerializeField] protected T prefab;
        [SerializeField] protected RectTransform content;
        public override void Init(IEnumerable<ItemRef> datas)
        {
            foreach (var inventoryItemUI in Items)
            {
                inventoryItemUI.Destroy();
            }
            Items.Clear();
            foreach (var data in datas)
            {
                var instance = Instantiate(prefab, content);
                instance.Init(data);
                Items.Add(instance);
            }
        }
    }
}