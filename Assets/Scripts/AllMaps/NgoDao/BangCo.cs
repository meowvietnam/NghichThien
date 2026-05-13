using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(BangCo), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(BangCo))]
public class BangCo : NgoDaoCombat
{
    public void ObserverEffect2(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (target == null) return;

        if (target.ngoDaoCombatsDynamic.Any(x => x.GetType() == this.GetType()))
        {
            ExecuteNgoDao2(caster, target, data);

        }
    }
    public void ExecuteNgoDao2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddDamageData(new List<int> { -target.allDamages[0] / 2 });
        target.RemoveNgoDaoCombatDynamic(this);
    }
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddNgoDaoCombatDynamic(this);
    }
    public override string GetDes()
    {
        return $"Mỗi hiệp, lần đầu nhận tổn thương giảm một nửa";
    }
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect2);
    }
}
