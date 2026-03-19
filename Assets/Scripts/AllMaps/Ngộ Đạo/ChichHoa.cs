using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ChichHoa), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ChichHoa))]
public class ChichHoa : NgoDaoCombat
{
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        CombatStatic.ThieuDot(caster, target);
    }

    public override string GetDes()
    {
        return $"Kết thúc hiệp, phát động một lần {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} cho đối thủ";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }
}
