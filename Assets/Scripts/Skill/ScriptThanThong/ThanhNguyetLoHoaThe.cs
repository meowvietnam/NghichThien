using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThanhNguyetLoHoaThe), menuName = "Skill/Thần Thông/" + nameof(ThanhNguyetLoHoaThe))]
public class ThanhNguyetLoHoaThe : ThanThong
{
    [SerializeField]
    private int percent = 20;
    [SerializeField]
    private int[] buffs = new int[]
 {
        1,1,1,1,1
 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect3);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0]; // ng gây dame
        CharacterCombat targetTurn = (CharacterCombat)data[1];// người nhận dame

        // hiệu ứng giảm bớt dame 
        if (targetTurn == null) return;

        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is ThanhNguyetLoHoaThe))
        {

            for (int i = 0; i < targetTurn.allDamages.Count; i++)
            {
                int item = targetTurn.allDamages[i];
                int dame = (int)(item * percent / 100f);
                targetTurn.allDamages[i] -= dame;

                targetTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, buffs[GetIndexLevelThanThong(targetTurn)]);
            }
                


        }


    }
    private void ObserverEffect3(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);



    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Bản thân nhận thần thông tổn thương giảm {GetColoredText($"{percent}%", "red")}. Nhưng mỗi lần nhận sát thương thì nhận phải {GetColoredText($"+{buffs[level - 1]} * X", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}";

    }
}
