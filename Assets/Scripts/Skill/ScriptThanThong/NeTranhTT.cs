using LongDH.LongDHModules;
using System.Linq;
using Network;
using Skill;
using UnityEngine;

public class NeTranhTT : ThanThong, IMemorySerializable<NeTranhTT>
{
    [SerializeField] protected int[] percentNeTranh;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);

        Observer.AddObserver(EventList.Combat.CHECK_NE_TRANH, ObserverEffectNeTranh, 1);
    }

    protected void ObserverEffectNeTranh(params object[] data)
    {
        CharacterCombat takeDameChar = (CharacterCombat)data[1];
        int index = (int)data[2];
        if (takeDameChar == null) return;
        if (!takeDameChar.specialThanThongImpactAfterPlaySkill.Any(x => x.GetType() == this.GetType())) return;
        if (takeDameChar.allDamages[index] == -1) return;
        if (CombatStatic.IsActiveWithPercent(percentNeTranh[GetIndexLevelThanThong(takeDameChar)]))
        {
            takeDameChar.allDamages[index] = -1;
            NeTranhSuccess(takeDameChar);
        }
    }

    protected virtual void NeTranhSuccess(CharacterCombat takeDameChar)
    {
    }

    public void Deserialize(IReader reader, ref NeTranhTT value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }
}