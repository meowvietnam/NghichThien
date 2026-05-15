using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DanBuffRemoveAfterTime), menuName = "Đan dược/Đan dược Combat/" + nameof(DanBuffRemoveAfterTime))]

public class DanBuffRemoveAfterTime : DanDuocBuffStartGame
{
    [SerializeField] private int afterTime = 4;
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        return caster.danDuocCombatsDynamic.Any(x => IsMatching(x));

    }
    public override void ExecuteDanDuoc(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.ExecuteDanDuoc(caster, target, data);
        if(caster.CurrentTurn > afterTime)
        {
            caster.RemoveDanDuocDynamic(GetDanDuocCombat(caster).type);
            caster.RemoveBuffAndDebuffCount(buffType, buff * GetDanDuocCombat(caster).count);
        }    

    }
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        base.InitPosCallExecute(caster);
        caster.danDuocCombatsDynamic.Add(GetDanDuocCombat(caster).type);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);

    }
}
