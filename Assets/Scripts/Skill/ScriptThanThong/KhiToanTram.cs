using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KhiToanTram), menuName = "Skill/Thần Thông/" + nameof(KhiToanTram))]

public class KhiToanTram : ThanThong
{
    [SerializeField] private int[] buff;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, buff[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    
 
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Bản thân gỡ ra {GetColoredText($"{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}";
    }
}
