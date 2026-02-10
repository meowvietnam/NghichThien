using System.Collections.Generic;
using UnityEngine;

namespace LuyenDan.Inventory
{
    public class ItemListUI : MonoBehaviour
    {
        private List<InventoryItemUI> items = new List<InventoryItemUI>();
        [SerializeField] private InventoryItemUI prefab;
        [SerializeField] private RectTransform content;

        public void Init(IEnumerable<IInventoryItemData> datas)
        {
            foreach (var data in datas)
            {
                var instance = Instantiate(prefab, content);
                instance.Init(data);
                items.Add(instance);
            }
        }

        public void Filter(Tier tier)
        {
            foreach (var item in items)
            {
                var active = item.Tier == tier;
                item.SetActive(active);
            }
        }
        public void NoFilter()
        {
            foreach (var item in items)
            {
                item.SetActive(true);
            }
        }
    }
}