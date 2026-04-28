using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TrongKiemQuyet))]

public class TrongKiemQuyet : CongPhap
{
    [SerializeField] private int[] buff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.TieuTanLinhKhi(caster.GetRandomLinhKhiConLai(), 1);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, buff[CurrentLevelCongPhapInCombat(caster) - 1]);


    }
    
    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, ngẫu nhiên tiêu tán 1 điểm linh khí thu hoạch được {GetColoredText($"+{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}.";
    }


 

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);

    }
}
