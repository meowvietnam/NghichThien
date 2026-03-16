using LongDH.LongDHModules;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ChuHaiQuyet))]

public class ChuHaiQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
        1, 3 , 6,14, 36
       };
   
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        RemoveCongPhap(caster);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);



    }
    public virtual void ExecuteParams2(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        // Tìm trong danh sách trang bị của caster
        if (GetCongPhapMono(caster.congPhapEquip) != null)
        {
            Execute2(caster, target, data);

        }
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        CombatStatic.ThieuDot(caster, target);



    }
    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}. Khi kết thúc hiệp, phát động trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} của đối thủ";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams2);

    }
}
