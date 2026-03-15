using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ChanLongCuuBien))]

public class ChanLongCuuBien : ThoNapCongPhap
{
    [SerializeField]
    private int[] buffValues2 = new int[]
           {
            0, 0 , 1,1, 1
           };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


    }

    public override string GetDesc(int level)
    {
        return $"{GetDesc(level)}. Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanLongChiHuyet)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanLongChiHuyet, buffValues2[CurrentLevelCongPhapInCombat(caster) - 1]);

    }
}
