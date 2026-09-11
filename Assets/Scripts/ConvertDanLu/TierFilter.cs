using UnityEngine;
using UnityEngine.UI;

namespace ConvertDanLu
{
    /// <summary>
    /// A filter button that filters the Bag by Tier.
    /// Mirrors ConvertElementalSkill.TierFilter adapted for IDanItemData.
    /// </summary>
    public class TierFilter : BaseFilter
    {
        [SerializeField] private Base.Tier tier;

        public override bool Filter(ItemBag item)
        {
            return item.DanData.ItemData.Tier == tier;
        }
    }
}
