using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Skill;

[CreateAssetMenu(fileName = nameof(DaiTuDiThuat), menuName = "Skill/Thần Thông/" + nameof(DaiTuDiThuat))]

public class DaiTuDiThuat : ThanThong
{
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        List<int> dames = (List<int>)data[2];
        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is DaiTuDiThuat))
        {
            int count = currentTurn.specialThanThongImpactAfterPlaySkill.Count(x => x is DaiTuDiThuat);
            for (int i = 0; i < count; i++)
            {
                target.AddDamageData(dames);
            }
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);


        }


    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Lần tiếp theo sử dụng thần thông tổn thương gấp đôi (Có thể cộng dồn)";

    }
}
