using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThienTinhHoaHaiQuyet))]

public class ThienTinhHoaHaiQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1 , 2 , 3 ,5 , 7
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int currentBuff = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        int dame = caster.totalKhi * currentBuff;
        target.TakeDameByStatic(caster, new System.Collections.Generic.List<int> { dame }, true, eTypeSkill);

    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, đối thủ nhận {GetColoredText($"+{buffValues[level - 1]}*X", "red")} điểm sát thương (X là tổng số linh khí của bản thân)";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);

    }
}
