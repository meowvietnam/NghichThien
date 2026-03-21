using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KhinhKiem), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KhinhKiem))]
public class KhinhKiem : NgoDaoCombat
{
    [SerializeField] private int buff = 2;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (BaseSkill.CheckIsEqualETypeSkill(caster,eTypeRequire))
        {
            target.AddDamageData(new List<int> { buff });
        }
    }

    public override string GetDes()
    {
        return $"Sử dụng kiếm hệ thần thông sát thương {BaseSkill.GetColoredText($"+{buff}", "red")}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }
}
