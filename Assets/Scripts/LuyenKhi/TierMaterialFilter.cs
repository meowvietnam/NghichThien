using Base;
using UnityEngine;

namespace LuyenKhi
{
    public class TierMaterialFilter : MaterialFilter
    {
        [SerializeField] private Tier tier;
        public override bool Filter(Material item)
        {
            return item.Tier == tier;
        }
    }
}