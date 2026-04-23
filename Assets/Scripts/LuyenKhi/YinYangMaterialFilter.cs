using UnityEngine;

namespace LuyenKhi
{
    public class YinYangMaterialFilter : MaterialFilter
    {
        [SerializeField] private YinYangType type;
        public override bool Filter(Material item)
        {
            return item.YinYangType == type;
        }
    }
}