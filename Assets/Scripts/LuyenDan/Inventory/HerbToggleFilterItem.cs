using UnityEngine;

namespace LuyenDan.Inventory
{
    public class HerbToggleFilterItem : ToggleFilterItem
    {
        [SerializeField] private HerbType herbType;
        [SerializeField] private HerbItemListUI target;
        protected override void OnValueChanged(bool isOn)
        {
            base.OnValueChanged(isOn);
            if (isOn)
            {
                target.Filter(herbType);
            }
        }
    }
}
