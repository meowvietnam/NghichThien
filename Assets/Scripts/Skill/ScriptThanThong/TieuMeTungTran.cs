using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TieuMeTungTran), menuName = "Skill/Thần Thông/" + nameof(TieuMeTungTran))]

public class TieuMeTungTran : NeTranhTT
{
    [SerializeField] private float delayTimeEf = 2f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        if (prefabEffectSkill != null)
        {
            ShowSkillEffectWorld(caster, delayTimeEf);
        }
    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.TranPhap)}. Bản thân có {GetColoredText($"{percentNeTranh[level - 1]}%", "red")} né tránh sát thương";
    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.Id == this.Id)) && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }
    protected override void NeTranhSuccess(CharacterCombat takeDameChar)
    {


    }

    
}
