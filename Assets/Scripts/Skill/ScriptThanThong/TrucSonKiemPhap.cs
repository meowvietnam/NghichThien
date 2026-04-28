using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TrucSonKiemPhap), menuName = "Skill/Thần Thông/" + nameof(TrucSonKiemPhap))]

public class TrucSonKiemPhap : ThanThong , BaseSkill.LienKich
{
    [SerializeField]
    private int[] dames = new int[]
      {
        2,4,6,12,30
      };
    [SerializeField]
    private int[] buffValues = new int[]
     {
        1,2,3,6,11
     };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        TakeDame(caster, target, dames, false);
    }
    private void TakeDame(CharacterCombat caster, CharacterCombat target, int[] dames,bool isCongDon = false)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new List<int> { dame }, eTypeSkill , isCongDon);
        caster.Healing(dame);
    }    
    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        TakeDame(caster, target, buffValues, true);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương hồi một lượng HP tương đương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Thuy)} hệ thần thông thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";

    }
}
