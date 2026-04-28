using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThoNapCongPhap))]

public class ThoNapCongPhap : CongPhap
{
    [SerializeField]
    private int[] buffValues;
    public override void EquipCongPhap(CharacterStatsSecure statsCurrent , int daiCanhGioiCongPhap)
    {

        statsCurrent.hpMax += buffValues[daiCanhGioiCongPhap - 1];
    }
    public override void UnEquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {

        statsCurrent.hpMax -= buffValues[daiCanhGioiCongPhap - 1];
        if(statsCurrent.hp >= statsCurrent.hpMax)
        {
            statsCurrent.hp -= buffValues[daiCanhGioiCongPhap - 1];

        }

    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        return;


    }

    public override string GetDesc(int level)
    {
        return $"HP tối đa {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        return;

    }
}
