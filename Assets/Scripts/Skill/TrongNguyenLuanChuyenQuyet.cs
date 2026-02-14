using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TrongNguyenLuanChuyenQuyet))]
public class TrongNguyenLuanChuyenQuyet : CongPhap
{
    [SerializeField] private int[] sucTheAndHoTrao;
    [SerializeField] private int[] bonusBuff;
    [SerializeField] private int require = 5;


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int level = CurrentLevelCongPhapInCombat(caster);
        if (level == 0) return;
        int buff = sucTheAndHoTrao[level - 1];

        foreach (var item in caster.thanThongUseThisTurn)
        {
            int totalCountLinhKhiTieuTan = 0;
            foreach (var l in item.linhKhiTieuTan)
            {
                totalCountLinhKhiTieuTan += l.count;
            }
            if(totalCountLinhKhiTieuTan >= require)
            {
                buff += bonusBuff[level - 1];
            }    
        }
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, buff);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao,buff);

    }
    public override string GetDesc(int level)
    {
        return $"Khi kết thúc hiệp, thu hoạch được {GetColoredText($"+{sucTheAndHoTrao[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}. Mỗi lần sử dụng thần thông có tiêu hao ít nhất {GetColoredText($"{require} điểm", "red")} linh khí thì thu hoạch được thêm {GetColoredText($"+{bonusBuff[level - 1]}", "red")} tầng";

    }



    public override void InitPosCallExecute(CharacterCombat caster)
    {

        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ExecuteParams);
    }

}
