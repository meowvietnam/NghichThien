using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyenThanhThienDienLuc))]
public class HuyenThanhThienDienLuc : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
            1, 2 , 3, 4, 5
    };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

    }

    public override string GetDesc(int level)
    {
        return $"Mỗi hiệp hấp thụ linh khí {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.totalKhiRandom += buffValues[CurrentLevelCongPhapInCombat(caster) - 1];

    }
}
