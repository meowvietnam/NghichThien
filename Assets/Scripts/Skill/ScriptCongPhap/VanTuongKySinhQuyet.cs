using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(VanTuongKySinhQuyet))]

public class VanTuongKySinhQuyet : CongPhap
{
    [SerializeField] private int[] buffValues;
    [SerializeField] private int[] percentActive;
    [SerializeField] private int require = 3;
    [SerializeField] private ThanThong thanThongActive;



    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        int trienNhieu = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieu);
        RemoveCongPhap(caster);


    }
    public virtual void ExecuteParams2(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        // Tìm trong danh sách trang bị của caster
        if (GetCongPhapMono(caster.congPhapEquip) != null)
        {
            Execute2(caster, target, data);

        }
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTans = (List<LinhKhiTieuTan>)data[3];
        int mocLinhKhi = 0;
        foreach (var item in linhKhiTieuTans)
        {
            if (item.eLinhKhi == ELinhKhi.Moc)
            {
                mocLinhKhi += item.count;
            }
        }
        int percent = percentActive[CurrentLevelCongPhapInCombat(caster) - 1];
        if (caster.IsSkillTichLuyCanUse(this, mocLinhKhi, require))
        {
           if(CombatStatic.IsActiveWithPercent(percent))
           {
                Observer.Notify(EventList.Combat.PLAY_SKILL, caster, target, thanThongActive, new List<LinhKhiTieuTan>());
           }

        }

    }
    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, đối phương nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}. Mỗi tích lũy sử dụng {GetColoredText($"{require}", "red")} điểm {GetTextTypeSkill()} hệ linh khí, có {GetColoredText($"{percentActive[level - 1]}%", "red")} phóng thích thần thông [Ký Sinh Dẫn Huyết Thuật]";

    }



    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams2);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);

    }
}
