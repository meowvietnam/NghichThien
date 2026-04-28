using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(DoatLinhQuyet))]
public class DoatLinhQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
            2, 3 ,4,5, 6
       };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        var buff = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        var danhCap = target.CurrentLinhKhi > buff ? buff : target.CurrentLinhKhi;
        for (int i = 0; i < danhCap; i++)
        {
            var linhKhi = target.GetRandomLinhKhiConLai();
            caster.HapThuLinhKhi(linhKhi, 1);
            target.TieuTanLinhKhi(linhKhi, 1);
        }

    }

    public override string GetDesc(int level)
    {
        return $"Giai đoạn hấp thụ linh khí. Từ bỏ hấp thụ {GetColoredText($"{buffValues[level - 1]}", "red")} điểm linh khí thì thay vào đó đánh cắp {GetColoredText($"{buffValues[level - 1]}", "red")} điểm linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);
        caster.totalKhiRandom -= buffValues[CurrentLevelCongPhapInCombat(caster)-1];
    }
}
