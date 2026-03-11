using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NgocVanCong))]
public class NgocVanCong : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
            5, 9 , 18,36, 72
    };
   

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
    }
}
