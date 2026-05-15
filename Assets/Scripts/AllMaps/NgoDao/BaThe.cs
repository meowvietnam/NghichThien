using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(BaThe), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(BaThe))]

public class BaThe : NgoDaoCombat
{
    [SerializeField] private int buff = 30;
    public override void ObserverEffect(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (target == null) return;

        if (target.ngoDaoCombats.Any(x => x.Id == this.Id))
        {
            ExecuteNgoDao(caster, target, data);

        }


    }
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddDamageData(new List<int> { -target.allDamages[0] * buff / 100 });

    }

    public override string GetDes()
    {
        return $"Nhận tất cả tổn thương {BaseSkill.GetColoredText($"-{buff}%", "red")}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect);
    }
}
