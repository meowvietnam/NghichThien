using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TuDuongHonNguyenKinh))]
public class TuDuongHonNguyenKinh : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
            20, 30 , 50,75, 100
       };
   
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

       
    }
    public int GetPercent(CharacterCombat caster)
    {
        return buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
    }    
    public override string GetDesc(int level)
    {
        return $"Mỗi lần sử dụng [Vận Khí Thuật], có {GetColoredText($"{buffValues[level - 1]}%", "red")} hấp thụ gấp đôi linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
    }
}
