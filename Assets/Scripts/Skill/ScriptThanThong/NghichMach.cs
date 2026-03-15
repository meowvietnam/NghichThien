using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NghichMach), menuName = "Skill/Thần Thông/" + nameof(NghichMach))]
public class NghichMach : ThanThong
{
    [SerializeField] private int treKhi = 2;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, treKhi);
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ObserverEffect2);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Bản thân nhận phải {GetColoredText($"+{treKhi}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}. Trong hiệp này, sử dụng thần thông tiếp theo gây tổn thương thì bản thân được hồi lượng Hp bằng lượng sát thương gây ra";
    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        List<int> dames = (List<int>)data[2];
        if (currentTurn == null) return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is NghichMach))
        {
            List<NghichMach> listThisSkill = currentTurn.specialThanThongImpactAfterPlaySkill
                                 .OfType<NghichMach>()
                                 .ToList();
            int heal = listThisSkill.Count * dames.Sum();
            currentTurn.Healing(heal);
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);




        }




    }
    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);




    }
}
