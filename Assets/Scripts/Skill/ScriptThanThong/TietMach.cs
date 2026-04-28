using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = nameof(TietMach), menuName = "Skill/Thần Thông/" + nameof(TietMach))]
public class TietMach : ThanThong , BaseSkill.TuongDong
{
    [SerializeField] private int[] dameMain;


    [SerializeField] private int[] buffDame;

    [SerializeField] private int deBuff = 1;
    [SerializeField] protected float delayTimeEffect = 1f;

    [SerializeField] protected virtual EBuffAndDeBuff eBuffOrDebuff => EBuffAndDeBuff.TreKhi;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.CaculateDameByThanThong(caster,new List<int> { dameMain[GetIndexLevelThanThong(caster)]}, eTypeSkill);
        target.AddBuffAndDebuffCount(eBuffOrDebuff, deBuff);
        ShowSkillEffectWorld(target, delayTimeEffect);

    }

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        target.CaculateDameByThanThong(caster, new List<int> { buffDame[GetIndexLevelThanThong(caster)]}, eTypeSkill, true);
    }

    public override string GetDesc(int level)
    {
        return $"Gây ra {GetColoredText($"{dameMain[level - 1]} điểm", "red")} sát thương, đối phương nhận phải {GetKeyWordBuffOrDeBuff(eBuffOrDebuff)}. Nếu {GetKeyWord(EKeyWord.TuongDong)} đối phương nhận thêm {GetColoredText($"+{buffDame[level - 1]} điểm", "red")} sát thương";

    }
   

}
