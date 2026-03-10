using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThoDiemQuyet), menuName = "Skill/Thần Thông/" + nameof(ThoDiemQuyet))]

public class ThoDiemQuyet : ThanThong , BaseSkill.LienKich
{
    [SerializeField] protected int[] dames;
    [SerializeField] protected int[] thieuDots;


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        
        target.TakeDameByThanThong(caster, new List<int> { dames[GetIndexLevelThanThong(caster)] }, eTypeSkill, false);


    }

    
    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng Mộc hệ thần thông, thì đối thủ nhận phải {GetColoredText($"+{thieuDots[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, thieuDots[GetIndexLevelThanThong(caster)]);
    }
}
