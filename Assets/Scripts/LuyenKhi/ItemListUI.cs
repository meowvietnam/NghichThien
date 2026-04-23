using System.Collections.Generic;
using System.Linq;
using LuyenDan;
using UnityEngine;

namespace LuyenKhi
{
    public class ItemListUI : MonoBehaviour
    {
        public List<ItemBag> Items { get; private set; } = new();
        [SerializeField] protected ItemBag prefab;
        [SerializeField] protected RectTransform content;
        private List<IItemFilter<Material>> filters = new ();
        public void Init(IEnumerable<ItemRef> datas)
        {
            foreach (var data in datas)
            {
                var instance = Instantiate(prefab, content);
                instance.Init(data);
                Items.Add(instance);
            }
        }

        public void AddFilter(IItemFilter<Material> filter)
        {
            filters.Add(filter);
        }

        public void RemoveFilter(IItemFilter<Material> filter)
        {
            filters.Remove(filter);
        }

        public void Refresh()
        {
            foreach (var itemBag in Items)
            {
                var mat = itemBag.Material;
                var active = filters.All(x => x.Filter(mat));
                itemBag.SetActive(active);
            }
        }
    }
}