using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhiVanKinh))]
public class PhiVanKinh : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
            1, 2 , 3,4, 5
       };

    public override void EquipCongPhap(CharacterStats statsMax, CharacterStats statsCurrent, int level)
    {
        statsMax.tocDoBay += buffValues[level - 1];
        statsCurrent.tocDoBay += buffValues[level - 1];

    }
    public override void UnEquipCongPhap(CharacterStats statsMax, CharacterStats statsCurrent, int level)
    {
        statsMax.tocDoBay -= buffValues[level - 1];
        statsCurrent.tocDoBay -= buffValues[level - 1];

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
