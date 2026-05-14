using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TrongKiem), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TrongKiem))]
public class TrongKiem : NgoDaoCombat
{
    [SerializeField] private int buff = 10;
 
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddNgoDaoCombatDynamic(this);


    }
    public void Action(CharacterCombat caster, CharacterCombat target)
    {
        target.AddDamageData(new List<int> { target.allDamages[0] * buff / 100 });
        caster.RemoveNgoDaoCombatDynamic(this);
    }
    public void ObserverEffect2(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        if (caster == null) return;

        if (caster.ngoDaoCombats.Any(x => x.Id == this.Id))
        {
            if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire))
            {
                Action(caster, target);

            }


        }


    }
    public void ObserverEffect3(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        BaseSkill.ETypeSkill[] eTypeSkills = (BaseSkill.ETypeSkill[])data[3];

        if (caster == null) return;
        // kiểm tra dame được truyền vào có phải là mộc linh khí không
        if (caster.ngoDaoCombats.Any(x => x.Id == this.Id) && eTypeSkills != null && BaseSkill.CheckIsEqualETypeSkill(eTypeSkills, eTypeRequire))
        {
            Action(caster, target);

        }


    }
    public override string GetDes()
    {
        return $"Mỗi hiệp lần đầu tạo thành kiếm hệ tổn thương {BaseSkill.GetColoredText($"+{buff}%", "red")}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect2);
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME_BY_STATIC, ObserverEffect3);
    }
}
