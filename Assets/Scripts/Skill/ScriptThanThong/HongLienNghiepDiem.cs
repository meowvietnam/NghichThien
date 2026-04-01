using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HongLienNghiepDiem), menuName = "Skill/Thần Thông/" + nameof(HongLienNghiepDiem))]

public class HongLienNghiepDiem : ThanThong
{
    [SerializeField] protected int[] thieuDots;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, thieuDots[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    
    public override string GetDesc(int level)
    {
        return $"Khiến đối thủ nhận phải {GetColoredText($"+{thieuDots[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";

    }
}
