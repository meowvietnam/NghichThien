using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;

public abstract class NeTranhTT : ThanThong
{
    [SerializeField] protected int[] percentNeTranh;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);

        Observer.AddObserver(EventList.Combat.CHECK_NE_TRANH, ObserverEffectNeTranh,1);
    }

    public abstract override string GetDesc(int level);
   
    protected void ObserverEffectNeTranh(params object[] data)
    {
        CharacterCombat takeDameChar = (CharacterCombat)data[1];
        int index = (int)data[2];
        if (takeDameChar == null) return;
        if (!takeDameChar.specialThanThongImpactAfterPlaySkill.Any(x=>x.GetType() == this.GetType())) return;
        if (takeDameChar.allDamages[index] == -1) return;
        if (CombatStatic.IsActiveWithPercent(percentNeTranh[GetIndexLevelThanThong(takeDameChar)]))
        {
            takeDameChar.allDamages[index] = -1;
            NeTranhSuccess(takeDameChar);
        }    
        


    }
    protected abstract void NeTranhSuccess(CharacterCombat takeDameChar);
    
}
