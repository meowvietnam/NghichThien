using LongDH.LongDHModules;
using UnityEngine;

public abstract class DanDuocBuffDame : BaseDanDuocCombat
{
    [SerializeField] protected int buff = 1;

    public override void ExecuteDanDuoc(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        var danDuocCombat = GetDanDuocCombat(caster);
        target.AddDamageData(new System.Collections.Generic.List<int> { buff * danDuocCombat.count });
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }
}