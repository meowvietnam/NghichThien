using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ChiThuy), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(ChiThuy))]
public class ChiThuy : NgoDaoCombat
{
    [SerializeField] private int buff = 1;    
    
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        EBuffAndDeBuff type = (EBuffAndDeBuff)data[1];
        int newValue = (int)data[2];
        int currentValue = (int)data[3];
        //Mathf.Abs(newValue - currentValue)
        if (type == EBuffAndDeBuff.ChiThuy && newValue < currentValue) // remove buff
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, buff);
        }    
    }

    public override string GetDes()
    {
        return $"Mỗi lần tiêu hao {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.ADD_BUFF, ObserverEffect);
    }


}
