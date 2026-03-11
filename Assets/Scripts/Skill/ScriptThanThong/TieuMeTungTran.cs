using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TieuMeTungTran), menuName = "Skill/Thần Thông/" + nameof(TieuMeTungTran))]

public class TieuMeTungTran : NeTranhTT
{
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.TranPhap)}. Bản thân có {GetColoredText($"{percentNeTranh[level - 1]}%", "red")} né tránh sát thương";
    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.GetType() == this.GetType())))
        {
            return true;
        }
        return false;
    }
    protected override void NeTranhSuccess(CharacterCombat takeDameChar)
    {


    }

    
}
