using UnityEngine;

namespace LuyenKhi
{
    public class CultivationItemBagFilter : ItemBagFilter
    {
        [SerializeField] private CultivationElement type;
        public override bool Filter(ItemBag item)
        {
            return item.Material.CultivationElement == type;
        }
    }
}