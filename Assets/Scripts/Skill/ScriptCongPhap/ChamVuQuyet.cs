using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ChamVuQuyet))]

public class ChamVuQuyet : CongPhap
{
    [SerializeField] private int[] buffValues;



    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        int trungDoc = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, trungDoc);
        RemoveCongPhap(caster);

    }
    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, đối phương nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}";

    }



    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);
    }
}
