using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(BanCan), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(BanCan))]
public class BanCan : NgoDaoCombat
{
  

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu,out SecureInt trienNhieu))
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieu);
        }    
    }

    public override string GetDes()
    {
        return $"Kết thúc hiệp, đối thủ số tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)} gấp đôi";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }
}
