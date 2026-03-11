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
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.GetType() == this.GetType())))
        {
            return true;
        }
        return false;
    }
    public abstract void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target);

    public virtual void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x.GetType() == this.GetType()))
        {
            ExecuteTranPhap(currentTurn,targetTurn);

        }


    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.TranPhap)}. {GetDescTranPhapTT(level)}";

    }
    public abstract string GetDescTranPhapTT(int level);
}
