using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThuyLongDan), menuName = "Skill/Thần Thông/" + nameof(ThuyLongDan))]

public class ThuyLongDan : ThanThong , BaseSkill.LienKich
{
    [SerializeField] private int[] dame;
    [SerializeField] private int[] healValues;
    [SerializeField]
    private float delayTimeEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        target.CaculateDameByThanThong(caster, new List<int> { dame[GetIndexLevelThanThong(caster)] }, eTypeSkill);
        //ShowSkillEffect(target, delayTimeEffect);

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.Healing(healValues[GetIndexLevelThanThong(caster)]);

    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dame[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Kim)} hệ kĩ năng thì khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm HP";
    }

}
