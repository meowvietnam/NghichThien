using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguDocThucTamQuyet))]

public class NguDocThucTamQuyet : CongPhap
{
    [SerializeField] private int[] debuffValues;



    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int debuff = debuffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, debuff);



    }
    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, đối phương nhận phải {GetColoredText($"+{debuffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}";

    }



    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);
    }
}
