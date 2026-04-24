using System.Collections.Generic;
using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;

namespace LuyenKhi
{
    public class Bag : BaseBehaviour, IBackContext
    {
        [SerializeField] private ItemListUI itemListUI;
        [SerializeField] private GameObject selfObject;
        private ISelectItemBagCallback callback;
        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
            LuyenKhiPanel.Instance.SetBackContext(this);
        }
        public void Init(IEnumerable<ItemRef> items)
        {
            itemListUI.Init(items);
        }

        public void OpenWithCallback(ISelectItemBagCallback callback)
        {
            this.callback = callback;
            SetActive(true);
        }
        public void OnSelect(ItemBag itemBag)
        {
            callback?.Select(itemBag);
            SetActive(false);
        }

        public void Back()
        {
            SetActive(false);
        }
    }
}