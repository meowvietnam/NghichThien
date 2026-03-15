using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HoanAnhDien))]
public class HoanAnhDien : CPTieuHaoThoNguyen
{
    [SerializeField]
    private int[] buffValues = new int[]
        {
            8, 16 , 24,32, 40
        };



   
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)}. {base.GetDesc(level)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.AnhDich, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);

    }
}
