using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguLinhDien))]

public class NguLinhDien : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
        {
            3,6,12,24,48
        };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       

    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, triệu hồi {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.QuyCuu)} và thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.QuyCuu, 1);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Hon, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
    }
}
