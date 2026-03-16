using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThanUyThuc))]

public class ThanUyThuc : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
         {
            1, 1 , 2,2, 3
         };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int treKhi = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, treKhi);

        RemoveCongPhap(caster);


    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);

    }
}
