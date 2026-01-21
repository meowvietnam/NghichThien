using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class KimCuongTram : ThanThong , BaseSkill.LienKich
{
    [SerializeField] private int[] dame;
    [SerializeField] private int[] hapThu;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.TakeDameByThanThong(caster, new List<int> { dame[GetIndexLevelThanThong(caster)-1] });

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.HapThuLinhKhi(hapThu[GetIndexLevelThanThong(caster) - 1]);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dame[level-1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} nếu trước đó dùng Thổ hệ kĩ năng thì hấp thụ {GetColoredText($"+{hapThu}", "red")} điểm linh khí";
    }

   
}
