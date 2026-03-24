using UnityEngine;
using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
[CreateAssetMenu(fileName = nameof(ThuLaoThuat), menuName = "Skill/Thần Thông/" + nameof(ThuLaoThuat))]
public class ThuLaoThuat : ThanThong , BaseSkill.LienKich
{
    [SerializeField] private int[] dameBuffs;

    [SerializeField] private int[] trienNhieu;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.CaculateDameByThanThong(caster,new List<int> { dameBuffs[GetIndexLevelThanThong(caster)] }, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieu[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameBuffs[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Thuy)} hệ thần thông, làm cho đối phương nhận phải {GetColoredText($"+{trienNhieu[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}";
    }
}
