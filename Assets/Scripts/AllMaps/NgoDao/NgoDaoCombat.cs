using System.Linq;
using AllMaps.NgoDao;
using UnityEngine;

public abstract class NgoDaoCombat : NgoDao
{
    public abstract void InitPosCallExecute(CharacterCombat caster);
    public abstract void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data);


    public virtual void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (caster == null) return;

        if (caster.ngoDaoCombats.Any(x => x.GetType() == this.GetType()))
        {
            ExecuteNgoDao(caster, target,data);

        }


    }
    
}
