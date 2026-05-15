using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThienPhong), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ThienPhong))]

public class ThienPhong : NgoDaoCombat
{
    [SerializeField] private int buff = 25;
    [SerializeField] private int requireLinhKhi = 2;

    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (caster == null) return;

        if (caster.ngoDaoCombatsDynamic.Any(x => x.Id == this.Id))
        {
            ExecuteNgoDao(caster, target, data);

        }


    }
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        caster.RemoveNgoDaoCombatDynamic(this);
        caster.totalKhiRandom += requireLinhKhi;
    }

    public override string GetDes()
    {
        return $"Bắt đầu chiến đấu, hấp thụ linh khi số lượng giảm {BaseSkill.GetColoredText($"-{requireLinhKhi}", "red")} nhưng thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buff);
        caster.totalKhiRandom -= requireLinhKhi;
        caster.AddNgoDaoCombatDynamic(this);

    }
}
