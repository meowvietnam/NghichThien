using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(LoanDangThuat), menuName = "Skill/Thần Thông/" + nameof(LoanDangThuat))]

public class LoanDangThuat : ThanThong
{
    [SerializeField] private int[] dames;
    [SerializeField] private int[] dameBuffs;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        SecureInt trienNhieu = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu,out trienNhieu);
        dame += trienNhieu * dameBuffs[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        caster.Healing(dame);
        ShowSkillEffectWorld(target, delayTimeEffect);

    }

    
    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Đối phương mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)} thì tổn thương tăng lên {GetColoredText($"+{dameBuffs[level - 1]}", "red")}";

    }
}
