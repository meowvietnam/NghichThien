using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TinhHaKiemPhap), menuName = "Skill/Thần Thông/" + nameof(TinhHaKiemPhap))]

public class TinhHaKiemPhap : ThanThong, BaseSkill.LienKich
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        5,7,13,27,90
      };
    [SerializeField]
    private int[] buffValues2 = new int[]
     {
        1,2,3,6,11
     };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, buffValues[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, buffValues2[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKiem)}. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Kim)} hệ thần thông thì thu hoạch được {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKiem)}";

    }
}
