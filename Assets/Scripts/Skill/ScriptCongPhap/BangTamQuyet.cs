using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(BangTamQuyet))]
public class BangTamQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
            5, 10 , 15,20, 25
       };

    public override void EquipCongPhap(CharacterStats statsMax , CharacterStats statsCurrent, int level)
    {
        base.EquipCongPhap(statsMax,statsCurrent, level);
        statsMax.tamCanh += buffValues[level - 1];
        statsCurrent.tamCanh += buffValues[level - 1];

    }
    public override void UnEquipCongPhap(CharacterStats statsMax, CharacterStats statsCurrent, int level)
    {
        base.UnEquipCongPhap(statsMax,statsCurrent, level);
        statsMax.tamCanh -= buffValues[level - 1];
        statsCurrent.tamCanh -= buffValues[level - 1];


    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

    }

    public override string GetDesc(int level)
    {
        return $"Độn tốc {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {

    }
}
