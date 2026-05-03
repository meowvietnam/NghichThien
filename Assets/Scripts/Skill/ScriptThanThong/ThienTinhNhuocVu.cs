using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThienTinhNhuocVu), menuName = "Skill/Thần Thông/" + nameof(ThienTinhNhuocVu))]

public class ThienTinhNhuocVu : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
    {
        40,56,90,160,330
    };
    [SerializeField]
    private int[] tranRequires = new int[]
    {
        40,60,80,100,120
    };
    [SerializeField]
    private int[] damebuffs = new int[]
    {
        30,55,80,105,130
    };
    [SerializeField] protected float delayTimeEffect = 0.85f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        int damebuff = damebuffs[GetIndexLevelThanThong(caster)];

        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt  tran);
        int require = tranRequires[GetIndexLevelThanThong(caster)];

        int v = tran / require;
        dame += damebuff * v;
        
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, tran);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        ShowSkillEffectNoTarget(delayTimeEffect);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} mỗi tiêu hao {GetColoredText($"{tranRequires[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} tăng lên {GetColoredText($"{damebuffs[level - 1]}", "red")} điểm sát thương";

    }
}
