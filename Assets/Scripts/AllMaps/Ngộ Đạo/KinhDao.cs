using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KinhDao), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(KinhDao))]
public class KinhDao : NgoDaoCombat
{
    [SerializeField] private int buff = 30;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        EBuffAndDeBuff type = (EBuffAndDeBuff)data[1];
        int newValue = (int)data[2];
        int currentValue = (int)data[3];
        //Mathf.Abs(newValue - currentValue)
        if (type == EBuffAndDeBuff.KinhDao && newValue < currentValue) // remove buff
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanThuy, buff);
        }
    }

    public override string GetDes()
    {
        return $"Mỗi lần tiêu hao {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanThuy)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.ADD_BUFF, ObserverEffect);
    }

}
