using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DanDuocBuffDameWithELinhKhi), menuName = "Đan dược/Đan dược Combat/" + nameof(DanDuocBuffDameWithELinhKhi))]
public class DanDuocBuffDameWithELinhKhi : DanDuocBuffDame
{
    [SerializeField] private ELinhKhi elinhKhi;

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        return base.IsCanUse(caster, target) && BaseSkill.CheckIsEqualETypeSkill(caster, new BaseSkill.ETypeSkill[] { BaseSkill.GetETypeSkillWithLinhKhi(elinhKhi) });
    }
   
    public void ObserverEffect2(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        BaseSkill.ETypeSkill[] eTypeSkills = (BaseSkill.ETypeSkill[])data[3];
        if (caster == null) return;
        // kiểm tra dame được truyền vào có phải là mộc linh khí không
        if (base.IsCanUse(caster,target) && eTypeSkills != null && BaseSkill.CheckIsEqualETypeSkill(eTypeSkills, new BaseSkill.ETypeSkill[] { BaseSkill.GetETypeSkillWithLinhKhi(elinhKhi) }))
        {
            ExecuteDanDuoc(caster, target);

        }


    }
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        base.InitPosCallExecute(caster);
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME_BY_STATIC, ObserverEffect2);

    }
}
