using UnityEngine;

namespace LuyenDan.Inventory
{
    public class NoToggleFilterItem : ToggleFilterItem
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