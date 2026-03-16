using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThanhXaYeuQuyet))]

public class ThanhXaYeuQuyet : CongPhap
{
    [SerializeField] private int[] debuffValues;



    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(caster.thanThongCurrentUse.thanThong is ThanhXaKinh)
        {
            int debuff = debuffValues[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc,debuff);
        }    

        

    }
    public override string GetDesc(int level)
    {
        return $"Mỗi lần sử dụng thần thông [Thanh Xà Kình], đối phương nhận phải {GetColoredText($"+{debuffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}";

    }



    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);
    }
}
