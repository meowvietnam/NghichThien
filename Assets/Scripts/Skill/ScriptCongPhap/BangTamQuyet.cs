using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(BangTamQuyet))]
public class BangTamQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
            5, 10 , 15,20, 25
       };

    public override void EquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {

        statsCurrent.tamCanh += buffValues[daiCanhGioiCongPhap - 1];

    }
    public override void UnEquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {

        statsCurrent.tamCanh -= buffValues[daiCanhGioiCongPhap - 1];


    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

    }

    public override string GetDesc(int level)
    {
        return $"Tâm cảnh {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {

    }
}
