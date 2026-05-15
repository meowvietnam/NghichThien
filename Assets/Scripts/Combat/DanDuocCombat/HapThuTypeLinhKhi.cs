using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HapThuTypeLinhKhi), menuName = "Đan dược/Đan dược Combat/" + nameof(HapThuTypeLinhKhi))]

public class HapThuTypeLinhKhi : BaseDanDuocCombat
{
    [SerializeField] private ELinhKhi eLinhKhi;
    [SerializeField] private int hapThu = 1;
    public override void ExecuteDanDuoc(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.HapThuLinhKhi(eLinhKhi,hapThu * GetDanDuocCombat(caster).count);
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT,ObserverEffect);
    }
}
