using UnityEngine;
[CreateAssetMenu(fileName = nameof(HacUeCham), menuName = "Skill/Thần Thông/" + nameof(HacUeCham))]
public class HacUeCham : ThanThong
{
    [SerializeField] private int[] dames;
    [SerializeField] private int cham =1;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Cham, cham);

    }
    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Thu hoạch được {GetColoredText($"+{cham}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cham)}";

    }
}
