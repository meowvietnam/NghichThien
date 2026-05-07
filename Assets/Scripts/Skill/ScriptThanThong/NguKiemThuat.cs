using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NguKiemThuat), menuName = "Skill/Thần Thông/" + nameof(NguKiemThuat))]

public class NguKiemThuat : ThanThong , BaseSkill.TuongSinh
{

    [SerializeField]
    protected int[] dames = new int[]
    {
        2,4,8,16,40
    };
    [SerializeField]
    protected int[] buffValues = new int[]
    {
        1,2,3,4,5
    };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        ShowEffect(caster,target);
    }
    protected virtual void ShowEffect(CharacterCombat caster, CharacterCombat target)
    {
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }
    public virtual void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        int dame = buffValues[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill , true);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.TuongSinh)} Tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";

    }
}
