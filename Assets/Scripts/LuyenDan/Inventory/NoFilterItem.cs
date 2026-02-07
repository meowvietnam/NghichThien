using UnityEngine;

namespace LuyenDan.Inventory
{
    public class NoFilterItem : FilterItem
    {
        [SerializeField] private ItemListUI target;
        protected override void OnValueChanged(bool isOn)
        {
            base.OnValueChanged(isOn);
            if (!isOn) return;
            target.NoFilter();
        }
    }
}