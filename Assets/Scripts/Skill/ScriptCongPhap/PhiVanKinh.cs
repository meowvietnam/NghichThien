using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhiVanKinh))]
public class PhiVanKinh : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
            1, 2 , 3,4, 5
       };

    public override void EquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {

        statsCurrent.tocDoBay += buffValues[daiCanhGioiCongPhap - 1];

    }
    public override void UnEquipCongPhap(CharacterStatsSecure statsCurrent , int daiCanhGioiCongPhap)
    {

        statsCurrent.tocDoBay -= buffValues[daiCanhGioiCongPhap - 1];

    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

    }

    public override string GetDesc(int level)
    {
        return $"Độn tốc {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {

    }
}
