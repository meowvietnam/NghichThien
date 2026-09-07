using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConvertDanLu
{
    /// <summary>
    /// Scrollable list of ItemBag prefabs shown inside the Bag panel.
    /// Supports filtering (hide items with count == 0 or matching tier filters).
    /// </summary>
    public class ItemList : BaseBehaviour, IFilter
    {
        [SerializeField] private ItemBag itemPrefab;
        [SerializeField] private Transform content;

        private readonly List<ItemBag> items = new();
        private readonly List<IFilter> filters = new();

        public void Init(IEnumerable<DanItemData> danItemDatas)
        {
            foreach (var itemBag in items)
            {
                itemBag.DestroySelf();
            }

            items.Clear();
            foreach (var data in danItemDatas)
            {
                var item = Instantiate(itemPrefab, content);
                item.SetData(data);
                items.Add(item);
            }
        }

        public void ApplyFilterOn(ItemBag item)
        {
            var active = filters.All(f => f.Filter(item));
            item.SetActive(active);
        }

        public void AddFilter(IFilter filter)    => filters.Add(filter);
        public void RemoveFilter(IFilter filter) => filters.Remove(filter);

        public void ClearFilters()
        {
            filters.Clear();
            AddFilter(this); // default: hide items with count == 0
        }

        public void UpdateFilter()
        {
            foreach (var item in items)
            {
                ApplyFilterOn(item);
            }
        }

        public ItemBag FindItem(ushort selfId)
        {
            return items.FirstOrDefault(x => x.DanData.ItemRef.Id == selfId);
        }

        // IFilter: hide items whose count dropped to 0
        public bool Filter(ItemBag item) => item.DanData.ItemRef.Count > 0;
    }
}
