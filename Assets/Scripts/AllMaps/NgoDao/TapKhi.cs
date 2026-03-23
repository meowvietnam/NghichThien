using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TapKhi), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TapKhi))]
public class TapKhi : NgoDaoCombat
{
    [SerializeField] private int buff = 2;
    [SerializeField] private int requireActive = 3;

   
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(caster.thanThongUseThisTurn.Count < requireActive)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, buff);
        }    
      
    }

    public override string GetDes()
    {
        return $"Kết thúc hiệp, nếu như trong hiệp sử dụng thần thông nhỏ hơn {BaseSkill.GetColoredText($"{requireActive}", "red")} lần thì thu hoạch được {BaseSkill.GetColoredText($"+{buff}", "red")} tầng {BaseSkill.GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)}";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
     

    }
}
