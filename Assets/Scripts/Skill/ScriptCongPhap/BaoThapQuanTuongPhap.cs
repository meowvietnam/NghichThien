using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(BaoThapQuanTuongPhap))]
public class BaoThapQuanTuongPhap : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,3,5,7,9

    };
    [SerializeField]
    private int[] buffValues2 = new int[]
    {
        5,10,15,20,25

    };
    public override void EquipCongPhap(CharacterStats statsMax, CharacterStats statsCurrent, int level)
    {
        statsMax.thanThuc += buffValues[level - 1];
        statsCurrent.thanThuc += buffValues[level - 1];

        statsMax.tamCanh += buffValues2[level - 1];
        statsCurrent.tamCanh += buffValues2[level - 1];
    }
    public override void UnEquipCongPhap(CharacterStats statsMax, CharacterStats statsCurrent, int level)
    {
        statsMax.thanThuc -= buffValues[level - 1];
        statsCurrent.thanThuc -= buffValues[level - 1];

        statsMax.tamCanh -= buffValues2[level - 1];
        statsCurrent.tamCanh -= buffValues2[level - 1];

    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        return;


    }

    public override string GetDesc(int level)
    {
        return $"Thần thức {GetColoredText($"+{buffValues[level - 1]}", "red")}. Tâm cảnh {GetColoredText($"+{buffValues2[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        return;

    }
}
