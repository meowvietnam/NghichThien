using UnityEngine;
[CreateAssetMenu(fileName = nameof(HuyenAmKinh), menuName = "Skill/Thần Thông/" + nameof(HuyenAmKinh))]
public class HuyenAmKinh : AmKinhTT
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        9,9,9,10,10
    };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddAmKinhTT(this, afterHiep);
        ShowSkillEffectParticleBase(target, delayTimeEffect, 5);

    }
    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong, buffValues[GetIndexLevelThanThong(target)]);
    }

    public override string GetDescAmKinhTT(int level)
    {
        return $"Đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}";
    }

}
