using System.Collections.Generic;
using System.Linq;
using LongDHModules;
using LuyenDan;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertDanLu
{
    /// <summary>
    /// Scrollable bag showing the player's owned đan lô (furnaces).
    /// Each FurnaceData instance is shown as a separate item (count = 1, identified by SelfId).
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
        /// Initialises the bag from a collection of FurnaceData (đan lô).
        /// Each furnace is represented as a unique ItemRef with SelfId.
        /// </summary>
        public void Init(IEnumerable<FurnaceData> furnaces)
        {
            SetEventButton(maskBtn, Back);
            var danItems = furnaces
                .Select(f =>
                {
                    var itemRef = f.CreateRefSelfID();
                    IDanItemData itemData = new FurnaceItemData(f);
                    return new DanItemData(itemRef, itemData);
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

        public ItemBag FindItem(ushort selfId)
        {
            return itemList.FindItem(selfId);
        }

        private void OnDestroy()
        {
            maskBtn.onClick.RemoveAllListeners();
        }
    }
}
