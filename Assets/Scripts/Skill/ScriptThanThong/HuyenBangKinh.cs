using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HuyenBangKinh), menuName = "Skill/Thần Thông/" + nameof(HuyenBangKinh))]

public class HuyenBangKinh : AmKinhTT
{
    [SerializeField] private int[] suongGias;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, suongGias[GetIndexLevelThanThong(target)]);
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddAmKinhTT(this, afterHiep);
        ShowSkillEffectWorld(target, delayTimeEffect);


    }

    public override string GetDescAmKinhTT(int level)
    {
        return $"Nhận phải {GetColoredText($"+{suongGias[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}";
    }


}
