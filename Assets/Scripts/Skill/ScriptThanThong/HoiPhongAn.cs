using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HoiPhongAn), menuName = "Skill/Thần Thông/" + nameof(HoiPhongAn))]

public class HoiPhongAn : ThanThong
{
   
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,3,7,20,40
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME, ObserverEffect);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect2);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (targetTurn == null) return;

        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is HoiPhongAn))
        {
            targetTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.SucLuc, 1);
        }



    }
    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];

        if (currentTurn == null) return;

        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);




    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Mỗi lần bị tổn thương thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucLuc)}";

    }
}
