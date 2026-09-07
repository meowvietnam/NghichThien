using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan.Inventory
{
    public class FilterGroup : MonoBehaviour
    {
        [SerializeField] private List<ToggleFilterItem> items;

        public void Init()
        {
            if (items != null && items.Count > 0)
            {
                var item = items[0];
                item.ForceSelect();
            }
        }
    }
}