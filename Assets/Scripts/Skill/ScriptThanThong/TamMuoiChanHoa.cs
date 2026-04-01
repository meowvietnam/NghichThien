using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TamMuoiChanHoa), menuName = "Skill/Thần Thông/" + nameof(TamMuoiChanHoa))]

public class TamMuoiChanHoa : ThanThong , BaseSkill.TuongDong
{

    [SerializeField] protected int[] dames;
    [SerializeField] protected int[] thieuDots;
    [SerializeField] protected int[] dameBuffs;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


        target.CaculateDameByThanThong(caster, new List<int> { dames[GetIndexLevelThanThong(caster)] }, eTypeSkill);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, thieuDots[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        target.CaculateDameByThanThong(caster, new List<int> { dameBuffs[GetIndexLevelThanThong(caster)] }, eTypeSkill, true);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Đồng thời, đối thủ nhận phải {GetColoredText($"+{thieuDots[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}. Nếu {GetKeyWord(EKeyWord.TuongDong)} thì tăng lên {GetColoredText($"+{dameBuffs[level - 1]}", "red")} điểm sát thương";

    }
}
