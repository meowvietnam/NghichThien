using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(VanAnhLuc))]
public class VanAnhLuc : CPTieuHaoThoNguyen
{
    [SerializeField]
    private int[] buffValues = new int[]
        {
            48, 128 , 240,384, 480
        };




    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)}. {base.GetDesc(level)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Anh, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);

    }
}
