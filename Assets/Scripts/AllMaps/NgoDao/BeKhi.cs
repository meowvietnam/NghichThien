using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(BeKhi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(BeKhi))]
public class BeKhi : NgoDaoCombat
{

    [SerializeField] private int buff = 5;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        EBuffAndDeBuff eBuffParam = (EBuffAndDeBuff)data[1];
        int newValue = (int)data[2];
        int oldValue = (int)data[3];
        if(eBuffParam == EBuffAndDeBuff.AnNac)
        {
            if (oldValue == 0 && newValue > 0)
            {
                caster.totalKhi += buff;
            }
            else if (newValue == 0 && oldValue > 0)
            {
                caster.totalKhi -= buff;

            }
        }    
    }

    public override string GetDes()
    {
        return $"Bản thân ở trạng thái {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnNac)} thì linh khí tối đa {BaseSkill.GetColoredText($"+{buff}", "red")}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.ADD_BUFF, ObserverEffect);
    }

}
