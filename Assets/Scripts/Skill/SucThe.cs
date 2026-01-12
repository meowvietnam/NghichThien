using System.Diagnostics;
using UnityEngine;
[CreateAssetMenu]
public class SucThe : ThanThong
{
    [SerializeField] private int sucThe = 33;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe,sucThe);
        ShowSkillEffect(caster, 1f);

    }

    public override string GetDesc()
    {
        return $"Bản thân thu hoạch được {GetColoredText($"+{sucThe}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}";
    }

  
}
