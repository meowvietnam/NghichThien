using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LongTuongThonHaiQuyet))]

public class LongTuongThonHaiQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1 , 2 , 3 ,4 , 5
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, linh khí tối đa {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        RemoveCongPhap(caster);
        caster.totalKhi += buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
    }


}
