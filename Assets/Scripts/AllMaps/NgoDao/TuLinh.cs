using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TuLinh), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TuLinh))]

public class TuLinh : NgoDaoCombat
{
    [SerializeField] private int buff = 4;

    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (caster == null) return;

        if (caster.ngoDaoCombatsDynamic.Any(x => x.GetType() == this.GetType()))
        {
            ExecuteNgoDao(caster, target, data);

        }


    }
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];

        if ((thanThong is TranPhapTT || thanThong.GetEKeywordSkillHave().Contains(BaseSkill.EKeyWord.TranPhap)))
        {
            caster.RemoveNgoDaoCombatDynamic(this);
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, buff);
        }
     
    }

    public override string GetDes()
    {
        return $"Bắt đầu chiến đấu, lần đầu bày ra {BaseSkill.GetKeyWord(BaseSkill.EKeyWord.TranPhap)} thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
        caster.AddNgoDaoCombatDynamic(this);

    }
}
