using UnityEngine;

namespace LuyenDan.Inventory
{
    public class TierFilterItem : FilterItem
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