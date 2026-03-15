using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LangVanQuyet))]
public class LangVanQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
        {
            1, 1 , 2,2, 3
        };
   
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       
    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, Hấp thụ thêm {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.HapThuLinhKhi(buffValues[CurrentLevelCongPhapInCombat(caster)-1]);
    }
}
