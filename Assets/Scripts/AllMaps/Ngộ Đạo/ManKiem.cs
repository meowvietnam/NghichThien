using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ManKiem), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ManKiem))]
public class ManKiem : NgoDaoCombat
{
    [SerializeField] private int buff = 20;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire))
        {
            Action(caster, target);

        }

    }
    public virtual void Action(CharacterCombat caster, CharacterCombat target)
    {
        target.AddDamageData(new List<int> { target.allDamages[0] * buff / 100 });
    }
   
    public void ObserverEffect3(params object[] data)
    {
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        BaseSkill.ETypeSkill[] eTypeSkills = (BaseSkill.ETypeSkill[])data[3];

        if (caster == null) return;
        // kiểm tra dame được truyền vào có phải là mộc linh khí không
        if (caster.ngoDaoCombats.Any(x => x.GetType() == this.GetType()) && eTypeSkills != null && BaseSkill.CheckIsEqualETypeSkill(eTypeSkills, eTypeRequire))
        {
            Action(caster, target);

        }


    }
    public override string GetDes()
    {
        return $"Tạo thành kiếm hệ tổn thương {BaseSkill.GetColoredText($"+{buff}%", "red")}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME_BY_STATIC, ObserverEffect3);
    }
}
