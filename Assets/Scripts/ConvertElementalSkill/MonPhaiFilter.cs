using System;
using Base;
using UnityEngine;

namespace ConvertElementalSkill
{
    public class MonPhaiFilter : BaseFilter
    {
        [SerializeField] private EMonPhai monPhai;

        public override bool Filter(ItemBag item)
        {
            var type = BaseSkill.MonPhaiToSkill(monPhai);
            return Array.IndexOf(item.SkillData.ItemData.ETypeSkills, type) >= 0;
        }
    }
}