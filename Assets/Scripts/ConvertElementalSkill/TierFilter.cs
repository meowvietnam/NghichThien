using System;
using UnityEngine;

namespace ConvertElementalSkill
{
    public class TierFilter : BaseFilter
    {
        [SerializeField] private BaseSkill.ETierSkill tier;

        public override bool Filter(ItemBag item)
        {
            return item.SkillData.ItemData.TierSkill == tier;
        }
    }
}