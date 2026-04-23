using Base;
using UnityEngine;

namespace LuyenKhi
{
    public class TierItemBagFilter : ItemBagFilter
    {
        [SerializeField] private Tier tier;
        public override bool Filter(ItemBag item)
        {
            return item.Material.Tier == tier;
        }
    }
}