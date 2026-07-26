using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ConvertElementalSkill
{
    public class ItemList : BaseBehaviour, IFilter
    {
        [SerializeField] private ItemBag itemPrefab;
        [SerializeField] private Transform content;
        private List<ItemBag> items = new();
        private List<IFilter> filters = new();
        public void Init(IEnumerable<SkillData> skillDatas)
        {
            foreach (var itemBag in items)
            {
                itemBag.DestroySelf();
            }
            items.Clear();
            foreach (var skillData in skillDatas)
            {
                var item = Instantiate(itemPrefab, content);
                item.SetData(skillData);
                items.Add(item);
            }
        }
        public void AddItem(SkillData skillData)
        {
            var item = Instantiate(itemPrefab, content);
            item.SetData(skillData);
            items.Add(item);
        }
        public void ApplyFilterOn(ItemBag item)
        {
            var active = true;
            foreach (var filter in filters)
            {
                active = active && filter.Filter(item);
            }
            item.SetActive(active);
        }
        public void AddFilter(IFilter filter)
        {
            filters.Add(filter);
        }
        public void RemoveFilter(IFilter filter)
        {
            filters.Remove(filter);
        }
        public void ClearFilters()
        {
            filters.Clear();
            AddFilter(this);
        }
        public void UpdateFilter()
        {
            foreach (var itemBag in items)
            {
                ApplyFilterOn(itemBag);
            }
        }
        public ItemBag FindItem(ushort id)
        {
            return items.FirstOrDefault(x => x.SkillData.ItemRef.Id == id);
        }

        public bool Filter(ItemBag item)
        {
            return item.SkillData.ItemRef.Count > 0;
        }

    }
}