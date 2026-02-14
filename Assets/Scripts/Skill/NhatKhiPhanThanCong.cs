using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NhatKhiPhanThanCong))]
public class NhatKhiPhanThanCong : CongPhap
{
    [SerializeField] private int[] hapThuLinhKhi;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int khi = hapThuLinhKhi[CurrentLevelCongPhapInCombat(caster) - 1];
        caster.totalKhi = 0;
        caster.totalKhiRandom += khi;
        caster.HapThuLinhKhi(khi);

    }

    public override string GetDesc(int level)
    {
        return $"Linh khí tối đa là 0, mỗi hiệp hấp thụ linh khí {GetColoredText($"+{hapThuLinhKhi[level - 1]}", "red")}";
    }


 

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ExecuteParams);
    }

}
