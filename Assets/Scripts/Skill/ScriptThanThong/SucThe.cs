using System.Diagnostics;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(SucThe), menuName = "Skill/Thần Thông/" + nameof(SucThe))]
public class SucThe : ThanThong
{
    [SerializeField] private int[] sucThe;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, sucThe[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, 1f);

    }

    public override string GetDesc(int level)
    {
        return $"Bản thân thu hoạch được {GetColoredText($"+{sucThe[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}";
    }

  
}
