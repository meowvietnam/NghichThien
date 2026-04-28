using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThauThachThuat), menuName = "Skill/Thần Thông/" + nameof(ThauThachThuat))]

public class ThauThachThuat : ThanThong
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,1,1,1,1
    };
    [SerializeField]
    private int[] dames = new int[]
    {
        4,8,13,27,32
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.PhaThe, buffValues[GetIndexLevelThanThong(caster)]);

    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.PhaThe)}";

    }
}
