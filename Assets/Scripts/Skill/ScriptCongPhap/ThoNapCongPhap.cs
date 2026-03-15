using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThoNapCongPhap))]

public class ThoNapCongPhap : CongPhap
{
    [SerializeField]
    private int[] buffValues;
    public override void EquipCongPhap(CharacterStats statsMax, CharacterStats statsCurrent, int level)
    {
        statsMax.hp += buffValues[level - 1];
    }
    public override void UnEquipCongPhap(CharacterStats statsMax, CharacterStats statsCurrent, int level)
    {
        statsMax.hp -= buffValues[level - 1];
        if(statsCurrent.hp >= statsMax.hp)
        {
            statsCurrent.hp -= buffValues[level - 1];

        }

    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        return;


    }

    public override string GetDesc(int level)
    {
        return $"HP tối đa {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        return;

    }
}
