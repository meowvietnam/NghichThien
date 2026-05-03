using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KhoaiKiem), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KhoaiKiem))]

public class KhoaiKiem : NgoDaoCombat
{
    [SerializeField] private int buff = 1;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        if (BaseSkill.CheckIsEqualETypeSkill(caster, eTypeRequire))
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKiemHiep, buff);
        }
    }

    public override string GetDes()
    {
        return $"Sử dụng kiếm hệ thần thông sát thương, thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NguKiemHiep)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }
}
