using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThonDiem), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ThonDiem))]
public class ThonDiem : NgoDaoCombat
{
 

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        EBuffAndDeBuff eType = (EBuffAndDeBuff)data[1];
        int newValue = (int)data[2];
        int oldValue = (int)data[3];

        if (eType == EBuffAndDeBuff.HoaDiem && newValue<=0) // thoát khỏi Hỏa Diễm
        {
            caster.Healing(oldValue);

        }   
        
    }

    public override string GetDes()
    {
        return $"Thoát khỏi trạng thái {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)} thì khôi phục HP một lượng tương đương số tầng";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.ADD_BUFF, ObserverEffect);
    }

}
