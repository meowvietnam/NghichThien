using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DanDuocBuffDameStartTurn),
    menuName = "Đan dược/Đan dược Combat/" + nameof(DanDuocBuffDameStartTurn))]
public class DanDuocBuffDameStartTurn : DanDuocBuffDame
{
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        return caster.danDuocCombatsDynamic.Any(x => IsMatching(x));
    }

    public override void ExecuteDanDuoc(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        var danDuocCombat = GetDanDuocCombat(caster);

        base.ExecuteDanDuoc(caster, target, data);
        caster.RemoveDanDuocDynamic(danDuocCombat.type);
    }

    public virtual void ObserverEffect2(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        if (caster == null) return;
        if (caster.danDuocCombats.Any(x => IsMatching(x.type)))
        {
            ExecuteDanDuoc2(caster, target, data);
        }
    }

    public void ExecuteDanDuoc2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (!IsCanUse(caster, target))
        {
            var danDuocCombat = GetDanDuocCombat(caster);
            caster.danDuocCombatsDynamic.Add(danDuocCombat.type);
        }
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        base.InitPosCallExecute(caster);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect2);
    }
}