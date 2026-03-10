using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KimCuongTram), menuName = "Skill/Thần Thông/" + nameof(KimCuongTram))]
public class KimCuongTram : ThanThong , BaseSkill.LienKich
{
    [SerializeField] private int[] dame;
    [SerializeField] private int[] hapThu;
    [SerializeField]
    private float delayTimeEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.TakeDameByThanThong(caster, new List<int> { dame[GetIndexLevelThanThong(caster)]}, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.HapThuLinhKhi(hapThu[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dame[level-1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} nếu trước đó dùng Thổ hệ kĩ năng thì hấp thụ {GetColoredText($"+{hapThu}", "red")} điểm linh khí";
    }

   
}
