using System.Linq;
using UnityEngine;

public abstract class TranPhapTT : ThanThong
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.GetType() == this.GetType())) && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }
    public abstract void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target);

    public virtual void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (caster == null) return;

        if (caster.specialThanThongImpactAfterPlaySkill.Any(x => x.GetType() == this.GetType()))
        {
            ExecuteTranPhap(caster,target);

        }


    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.TranPhap)}. {GetDescTranPhapTT(level)}";

    }
    public abstract string GetDescTranPhapTT(int level);
}
