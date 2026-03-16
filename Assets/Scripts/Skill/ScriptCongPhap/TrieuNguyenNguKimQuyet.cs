using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TrieuNguyenNguKimQuyet))]
public class TrieuNguyenNguKimQuyet : CongPhap
{
    [SerializeField] private int[] hoTraoBuff;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = caster.khiKim;
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, count * hoTraoBuff[CurrentLevelCongPhapInCombat(caster) - 1]);

    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, mỗi khi còn 1 điểm Kim linh khí thu hoạch được {GetColoredText($"+{hoTraoBuff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }


  
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);
    }

}
