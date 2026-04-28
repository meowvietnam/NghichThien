using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThanhXaKinh), menuName = "Skill/Thần Thông/" + nameof(ThanhXaKinh))]
public class ThanhXaKinh : ThanThong
{
    [SerializeField] private int[] trungDoc;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, trungDoc[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }



    public override string GetDesc(int level)
    {
        return $"Đối phương nhận phải {GetColoredText($"+{trungDoc[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}";
    }
}
