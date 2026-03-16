using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(SucLuc), menuName = "Skill/Thần Thông/" + nameof(SucLuc))]

public class SucLuc : ThanThong
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        2,2,2,2,2
    };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is SucLuc))
        {
            
           
            currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.SucLuc, buffValues[GetIndexLevelThanThong(currentTurn)]);
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);

        }


    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Hiệp sau, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucLuc)}";

    }
}
