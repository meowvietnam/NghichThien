using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(UHonLuc))]

public class UHonLuc : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
        {
            4,8,12,24,36
        };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       

    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Hon, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);

    }
}
