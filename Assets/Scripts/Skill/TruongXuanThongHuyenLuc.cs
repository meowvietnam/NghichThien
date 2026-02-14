using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TruongXuanThongHuyenLuc))]

public class TruongXuanThongHuyenLuc : CongPhap
{
    [SerializeField] private int[] buffValues;
    public override void EquipCongPhap(CharacterStats stats, int level)
    {
        base.EquipCongPhap(stats , level);
        stats.hp += buffValues[level-1];
    }
    public override void UnEquipCongPhap(CharacterStats stats, int level)
    {
        base.UnEquipCongPhap(stats,level);
        stats.hp -= buffValues[level-1];
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        



    }
    public override string GetDesc(int level)
    {
        return $"HP tối đa {GetColoredText($"+{buffValues[level - 1]}", "red")}";

    }



    public override void InitPosCallExecute(CharacterCombat caster)
    {

    }
}
