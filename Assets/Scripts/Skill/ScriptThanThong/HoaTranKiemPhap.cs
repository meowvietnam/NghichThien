using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = nameof(HoaTranKiemPhap), menuName = "Skill/Thần Thông/" + nameof(HoaTranKiemPhap))]

public class HoaTranKiemPhap : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
       {
        5,8,14,28,62
       };
    [SerializeField]
    private int[] buffValues = new int[]
     {
        1,1,1,1,1
     };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        TakeDame(caster, target, dames, false);
    }
    private void TakeDame(CharacterCombat caster, CharacterCombat target, int[] dames, bool isCongDon = false)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new List<int> { dame }, eTypeSkill, isCongDon);
    }
    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKiem, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NguKiem)}";

    }
}
