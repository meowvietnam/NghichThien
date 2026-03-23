using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TuLanh), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TuLanh))]
public class TuLanh : NgoDaoCombat
{
    [SerializeField] private int buff = 20;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int hpLost = (int)(caster.characterStatsTemp.hpMax - caster.characterStatsTemp.hp) * buff / 100;
        caster.Healing(hpLost);
    }

    public override string GetDes()
    {
        return $"Kết thúc hiệp, khôi phục một lương HP tương đương {BaseSkill.GetColoredText($"{buff}%", "red")} HP đã tổn thất";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }
}
