using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HuyetSatDan), menuName = "Đan dược/Đan dược Combat/" + nameof(HuyetSatDan))]

public class HuyetSatDan : DanDuocBuffDame
{
    [SerializeField] private int dameForCaster = 1;
    public void ObserverEffect2(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        if (caster == null) return;

        // kiểm tra dame được truyền vào có phải là mộc linh khí không
        if (IsCanUse(caster, target))
        {
            ExecuteDanDuoc2(caster, target);

        }


    }
    public void ExecuteDanDuoc2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        var danDuocCombat = GetDanDuocCombat(caster);
        caster.TakeDameByStatic(null,new System.Collections.Generic.List<int> { dameForCaster * danDuocCombat.count });
    }
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        base.InitPosCallExecute(caster);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect2);

    }
}
