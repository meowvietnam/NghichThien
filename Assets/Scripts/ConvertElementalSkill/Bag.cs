using System.Collections.Generic;
using System.Linq;
using LongDHModules;
using LuyenDan;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertElementalSkill
{
    public class Bag : BaseBehaviour
    {
        [SerializeField] private ItemList itemList;
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Button maskBtn;
        [SerializeField] private FilterGroup[] filterGroups;
        private ISelectedItemCallback callback;
        public ItemList ItemList => itemList;

        public void OpenWithCallback(ISelectedItemCallback callback)
        {
            this.callback = callback;
            SetActive(true);
        }

        public void OnSelected(ItemBag item)
        {
            SetActive(false);
            callback?.OnSelected(item);
        }

        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }

        private void Back()
        {
            SetActive(false);
        }

        public void Init(IEnumerable<ItemRef> items)
        {
            SetEventButton(maskBtn, Back);
            var ui = Pool.Get<UIManager>();
            var gameManager = Pool.Get<GameManager>();
            var skillDatas = items.Select(x =>
            {
                ISkillItemData itemData = x.Type == ItemType.ThanThong
                    ? new ThanThongItemData(x, gameManager, ui)
                    : new CongPhapItemData(x, gameManager, ui);
                return new SkillData(x, itemData);
            }).ToList();
            itemList.Init(skillDatas);
            itemList.ClearFilters();
            foreach (var filterGroup in filterGroups)
            {
                filterGroup.Init(itemList);
            }

            itemList.UpdateFilter();
        }

        public void AddItem(ItemRef item)
        {
            var currentItemBag = FindItem(item.Id);
            if (currentItemBag != null)
            {
                var itemRef = currentItemBag.SkillData.ItemRef;
                itemRef.SetCount(itemRef.Count + item.Count);
                itemList.ApplyFilterOn(currentItemBag);
            }
            else
            {
                var ui = Pool.Get<UIManager>();
                var gameManager = Pool.Get<GameManager>();
                ISkillItemData itemData = item.Type == ItemType.ThanThong
                    ? new ThanThongItemData(item, gameManager, ui)
                    : new CongPhapItemData(item, gameManager, ui);
                var skillData = new SkillData(item, itemData);
                itemList.AddItem(skillData);
                itemList.UpdateFilter();
            }
        }

        public ItemBag FindItem(ushort id)
        {
            return itemList.FindItem(id);
        }

        private void OnDestroy()
        {
            maskBtn.onClick.RemoveAllListeners();
        }
    }
}