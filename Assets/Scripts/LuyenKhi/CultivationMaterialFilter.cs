using UnityEngine;

namespace LuyenKhi
{
    public class CultivationMaterialFilter : MaterialFilter
    {
        [SerializeField] private CultivationElement type;
        public override bool Filter(Material item)
        {
            return item.CultivationElement == type;
        }
    }
}