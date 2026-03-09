using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThiThanThu), menuName = "Skill/Thần Thông/" + nameof(ThiThanThu))]

public class ThiThanThu : ThanThong, BaseSkill.ThanThuc
{
    [SerializeField]
    private int[] dames = new int[]
    {
        5,7,11,17,40
    };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,1,1,1,1
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }
    public void ExecuteThanThuc(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThanThuong, buffValues[GetIndexLevelThanThong(caster)]);
    }
    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.ThanThuc)} Đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThanThuong)}";

    }
}
