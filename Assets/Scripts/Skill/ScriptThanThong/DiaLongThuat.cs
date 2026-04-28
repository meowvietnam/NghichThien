using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DiaLongThuat), menuName = "Skill/Thần Thông/" + nameof(DiaLongThuat))]

public class DiaLongThuat : ThanThong, BaseSkill.LienKich
{
    [SerializeField]
    private int[] dameValues = new int[]
    {
        7,8,12,24,50
    };

    [SerializeField]
    private int[] buffValues = new int[]
    {
        3,6,9,15,30
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


        int dame = dameValues[GetIndexLevelThanThong(caster)];

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
       
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        int hoTrao = buffValues[GetIndexLevelThanThong(caster)];

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương.{GetKeyWord(EKeyWord.LienKich)}, nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";

    }
}
