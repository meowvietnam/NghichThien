using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DoanKim), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(DoanKim))]
public class DoanKim : NgoDaoCombat
{
    [SerializeField] private int buff = 6;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       if(caster.CurrentLinhKhi <=0)
       {
            caster.HapThuLinhKhi(ELinhKhi.Kim, buff);
       }    
    }

    public override string GetDes()
    {
        return $"Kết thúc hiệp, nêu không còn linh khí thì thu hoạch được {BaseSkill.GetColoredText($"{buff}", "red")} điểm {BaseSkill.GetTextLinhKhi(ELinhKhi.Kim)} hệ linh khí";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
    }

}
