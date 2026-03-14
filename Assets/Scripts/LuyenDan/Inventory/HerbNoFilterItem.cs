using UnityEngine;

namespace LuyenDan.Inventory
{
    public class HerbNoFilterItem : HerbFilterItem
    {
        [SerializeField] private HerbItemListUI target;
        protected override void OnClick()
        {
            target.NoFilter();
        }
    }
}
