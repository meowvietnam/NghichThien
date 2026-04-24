using UnityEngine;

namespace LuyenKhi
{
    public class YinYangItemBagFilter : ItemBagFilter
    {
        [SerializeField] private YinYangType type;
        public override bool Filter(ItemBag item)
        {
            return item.Material.YinYangType == type;
        }
    }
}