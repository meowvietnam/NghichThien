using Base;
using UnityEngine;

namespace LuyenDan.Inventory
{
    public class TierToggleFilterItem : ToggleFilterItem
    {
        [SerializeField] private ItemListUI target;
        [SerializeField] private Tier tier;
        protected override void OnValueChanged(bool isOn)
        {
            base.OnValueChanged(isOn);
            if(!isOn) return;
            target.Filter(tier);
        }
    }
}