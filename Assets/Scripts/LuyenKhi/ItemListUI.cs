using System.Collections.Generic;
using System.Linq;
using LuyenDan;
using UnityEngine;

namespace LuyenKhi
{
    public class ItemListUI : MonoBehaviour, IItemFilter<ItemBag>
    {
        public List<ItemBag> Items { get; private set; } = new();
        [SerializeField] protected ItemBag prefab;
        [SerializeField] protected RectTransform content;
        private List<IItemFilter<ItemBag>> filters = new ();
        public void Init(IEnumerable<ItemRef> datas)
        {
            foreach (var data in datas)
            {
                var instance = Instantiate(prefab, content);
                instance.Init(data);
                Items.Add(instance);
            }
            AddFilter(this);
            Refresh();
        }

        public void AddFilter(IItemFilter<ItemBag> filter)
        {
            filters.Add(filter);
        }

        public void RemoveFilter(IItemFilter<ItemBag> filter)
        {
            filters.Remove(filter);
        }

        public void Refresh()
        {
            foreach (var itemBag in Items)
            {
                var active = filters.All(x => x.Filter(itemBag));
                itemBag.SetActive(active);
            }
        }
        public bool Filter(ItemBag item)
        {
            return item.ItemRef.Count > 0;
        }
    }
}