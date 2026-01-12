using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class KimCuongTram : ThanThong , BaseSkill.LienKich
{
    [SerializeField] private int dame = 17;
    [SerializeField] private int hapThu = 1;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.TakeDameCaculate(caster, new List<int> { dame });

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.HapThuLinhKhi((ELinhKhi)Random.Range(1,6), hapThu);
    }

    public override string GetDesc()
    {
        return $"Tạo thành {GetColoredText($"{dame}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} nếu trước đó dùng Thổ hệ kĩ năng thì hấp thụ {GetColoredText($"+{hapThu}", "red")} điểm linh khí";
    }

   
}
