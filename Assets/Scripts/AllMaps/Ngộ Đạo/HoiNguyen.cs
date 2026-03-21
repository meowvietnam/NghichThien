using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HoiNguyen), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(HoiNguyen))]
public class HoiNguyen : NgoDaoCombat
{
    [SerializeField] private int buff = 5;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.Healing(buff);
    }

    public override string GetDes()
    {
        return $"Mỗi sử dụng thần thông gây tổn thương, khôi phục {BaseSkill.GetColoredText($"+{buff}", "red")} điểm HP";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
    }
}
