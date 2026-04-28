using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KinhHonChu), menuName = "Skill/Thần Thông/" + nameof(KinhHonChu))]

public class KinhHonChu : ThanThong , BaseSkill.ThanThuc
{
    [SerializeField]
    private int[] dames = new int[]
    {
        3,4,6,12,24
    };
    
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }

    public void ExecuteThanThuc(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByStatic(caster, new System.Collections.Generic.List<int> { dame },true, eTypeSkill);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.ThanThuc)} thì tổn thương gấp đôi";

    }
}
