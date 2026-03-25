using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThuyLieuThuat), menuName = "Skill/Thần Thông/" + nameof(ThuyLieuThuat))]

public class ThuyLieuThuat : ThanThong
{
    [SerializeField] private int[] healValues;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int heal = healValues[GetIndexLevelThanThong(caster)];
        int remainHeal = heal + (int)caster.characterStatsTemp.hp - (int)caster.characterStatsTemp.hpMax ;
        if(remainHeal > 0)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao,remainHeal);
        }
        caster.Healing(heal);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }
    public override string GetDesc(int level)
    {
        return $"Khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm Hp. Nếu đã đạt tới Hp tối đa lượng dư thừa con lại sẽ chuyển hóa thành {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }
}
