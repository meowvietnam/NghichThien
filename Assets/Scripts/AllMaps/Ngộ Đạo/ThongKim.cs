using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThongKim), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ThongKim))]

public class ThongKim : NgoDaoCombat
{
 

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       if(target.characterStatsTemp.hp < target.characterStatsTemp.hpMax * 50/100)
       {
            target.AddDamageData(new List<int> { target.allDamages[0] });
       }    
       else
       {
            float percentHpLost = (target.characterStatsTemp.hpMax - target.characterStatsTemp.hp) / target.characterStatsTemp.hpMax;
            target.AddDamageData(new List<int> { (int)(target.allDamages[0] * percentHpLost) });

        }
    }

    public override string GetDes()
    {
        return $"Đối thủ mỗi tổn thất 1% lượng máu, nhận tổn thương cũng tăng lên 1%. Nếu đối thủ HP thấp hơn 50% thì nhận tổn thương gấp đôi";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect);

    }

}
