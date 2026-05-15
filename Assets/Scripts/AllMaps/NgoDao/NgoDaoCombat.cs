using System.Linq;
using AllMaps.NgoDao;
using Network;
using UnityEngine;

public class NgoDaoCombat : NgoDao , IStaticCombatEffect, IMemorySerializable<NgoDaoCombat>
{
    public virtual void InitPosCallExecute(CharacterCombat caster) {}
    public virtual void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data) {}


    public virtual void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (caster == null) return;

        if (caster.ngoDaoCombats.Any(x => x.Id == this.Id))
        {
            ExecuteNgoDao(caster, target,data);

        }
    }

    public void Deserialize(IReader reader, ref NgoDaoCombat value)
    {
        NgoDao baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }
}
