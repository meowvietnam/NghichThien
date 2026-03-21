using UnityEngine;
[CreateAssetMenu(fileName = nameof(PheHonQuyet), menuName = "Skill/Thần Thông/" + nameof(PheHonQuyet))]

public class PheHonQuyet : ThanThong, BaseSkill.ThanThuc
{
    [SerializeField]
    private int[] dames = new int[]
     {
        4,6,10,20,40
     };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        2,2,2,2,2
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }

    public void ExecuteThanThuc(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Hon,out SecureInt  hon);
        int dame = hon * buffValues[GetIndexLevelThanThong(caster)];
        target.TakeDameByStatic(caster, new System.Collections.Generic.List<int> { dame }, true, eTypeSkill);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.ThanThuc)} Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";

    }
}
