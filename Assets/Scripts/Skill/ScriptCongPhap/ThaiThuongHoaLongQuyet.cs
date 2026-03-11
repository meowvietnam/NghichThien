using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiThuongHoaLongQuyet))]
public class ThaiThuongHoaLongQuyet : ThoNapCongPhap
{
    
    [SerializeField] private int percent = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.Healing((int)(caster.characterStatsMax.hp*percent/100));
    }
   
    public override string GetDesc(int level)
    {
        return $"{base.GetDesc(level)}. Bắt đầu hiệp, Khôi phục {GetColoredText($"{percent}%", "red")} điểm HP tối đa";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ExecuteParams);

    }
}
