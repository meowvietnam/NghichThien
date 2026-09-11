using System.Collections.Generic;
using System.Linq;
using LongDHModules;
using LuyenDan;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertDanLu
{
    /// <summary>
    /// Scrollable bag showing the player's owned đan phương (recipes).
    /// Each recipe ItemRef is shown with its count.
    /// </summary>
    public class Bag : BaseBehaviour
    {
        [SerializeField] private ItemList itemList;
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Button maskBtn;
        [SerializeField] private FilterGroup[] filterGroups;

        private ISelectedItemCallback callback;
        public ItemList ItemList => itemList;

        public void OpenWithCallback(ISelectedItemCallback cb)
        {
            callback = cb;
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

        /// <summary>
        /// Initialises the bag from the player's recipe inventory (đan phương).
        /// </summary>
        public void Init(IEnumerable<ItemRef> recipes)
        {
            SetEventButton(maskBtn, Back);
            var danItems = recipes
                .Select(r =>
                {
                    IDanItemData itemData = new RecipeItemData(r);
                    return new DanItemData(r, itemData);
                })
                .ToList();

            itemList.Init(danItems);
            itemList.ClearFilters();
            foreach (var filterGroup in filterGroups)
            {
                filterGroup.Init(itemList);
            }

            itemList.UpdateFilter();
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
