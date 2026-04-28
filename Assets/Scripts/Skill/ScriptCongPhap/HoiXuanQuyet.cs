using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HoiXuanQuyet))]

public class HoiXuanQuyet : CongPhap
{
    [SerializeField] private int[] buffValues;
    [SerializeField] private int require = 5;



    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        List<LinhKhiTieuTan> linhKhiTieuTans = (List<LinhKhiTieuTan>)data[3];
        int mocLinhKhi = 0;
        foreach (var item in linhKhiTieuTans)
        {
            if(item.eLinhKhi == ELinhKhi.Moc)
            {
                mocLinhKhi += item.count;
            }    
        }
        if(caster.IsSkillTichLuyCanUse(this,mocLinhKhi,require))
        {
            int buff = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
            caster.Healing(buff);

        }


    }
    public override string GetDesc(int level)
    {
        return $"Mỗi tích lũy sử dụng {GetColoredText($"{require}", "red")} điểm {GetTextTypeSkill()} hệ linh khí, bản thân khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm Hp";

    }



    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);
    }
}
