using LongDH.LongDHModules;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DanDuocMienDich), menuName = "Đan dược/Đan dược Combat/" + nameof(DanDuocMienDich))]
public class DanDuocMienDich : BaseDanDuocCombat
{
    [SerializeField] private EBuffAndDeBuff buffType;
    [SerializeField] private int valueHiep = 3;

    public override void ExecuteDanDuoc(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.CurrentTurn <= valueHiep)
        {
            caster.buffAndDebuffCount.TryGetValue(buffType, out SecureInt value);
            caster.RemoveBuffAndDebuffCount(buffType, value);
        }
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.ADD_BUFF, ObserverEffect);
    }
}