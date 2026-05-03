using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TuKhi), menuName = "Skill/Thần Thông/" + nameof(TuKhi))]

public class TuKhi : ThanThong
{
    [SerializeField]
    private int buffValues = 2;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, buffValues);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }


    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} thu hoạch được {GetColoredText($"{buffValues}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)}";

    }
}
