using System;
using System.Linq;
using UnityEngine;

namespace ConvertElementalSkill
{
    public class TypeSkillFilter : BaseFilter
    {
        [SerializeField] private BaseSkill.ETypeSkill type;

        public override bool Filter(ItemBag item)
        {
            return Array.IndexOf(item.SkillData.ItemData.ETypeSkills, type) >= 0;
        }
    }
}