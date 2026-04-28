using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DamHonThuat), menuName = "Skill/Thần Thông/" + nameof(DamHonThuat))]

public class DamHonThuat : ThanThong
{

    [SerializeField]
    private int[] dames = new int[]
    {
        4,7,12,16,20
    };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        3,4,6,8,11
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);

        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.TakeDameByStatic(null, new System.Collections.Generic.List<int> { dame }, false, null, false);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Hon, buffValues[GetIndexLevelThanThong(caster)]);
    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương cho bản thân. Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)}";

    }
}
