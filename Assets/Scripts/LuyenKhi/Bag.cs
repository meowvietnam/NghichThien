using System.Collections.Generic;
using LuyenDan;
using LuyenDan.Inventory;
using UnityEngine;

namespace LuyenKhi
{
    public class Bag : BaseBehaviour
    {
        [SerializeField] private ItemListUI itemListUI;
        public void Init(IEnumerable<ItemRef> items)
        {
            itemListUI.Init(items);
        }
    }
}