using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DanDuocGiamDame), menuName = "Đan dược/Đan dược Combat/" + nameof(DanDuocGiamDame))]
public class DanDuocGiamDame : BaseDanDuocCombat
{
    [SerializeField] private int buff = 1;

    public override void ExecuteDanDuoc(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddDamageData(new System.Collections.Generic.List<int>
            { -buff * GetDanDuocCombat(target).count }); // tarrget laf ng nhan dame
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }

    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        if (target == null) return;
        if (IsCanUse(target, caster))
        {
            ExecuteDanDuoc(caster, target, data);
        }
    }
}