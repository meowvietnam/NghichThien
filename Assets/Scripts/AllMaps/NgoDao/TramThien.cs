using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TramThien), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TramThien))]

public class TramThien : ManKiem
{
    public override void Action(CharacterCombat caster, CharacterCombat target)
    {
        if(target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao,out SecureInt hoTrao))
        {
            base.Action(caster, target);
        }    
    }
    public override string GetDes()
    {
        return $"Nếu đối thủ có {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}, {base.GetDes()}";
    }
}
