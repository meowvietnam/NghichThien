using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(KimHongKiemQuyet))]
public class KimHongKiemQuyet : CongPhap
{
    [SerializeField] private int[] sucTheBuff;
    [SerializeField] private int[] percentX2Dame;
    [SerializeField] private int requireLinhKhi =4 ;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTans = (List<LinhKhiTieuTan>)data[3];
        int kimLinhKhi = 0;
        foreach (var item in linhKhiTieuTans)
        {
            if (item.eLinhKhi == ELinhKhi.Kim)
            {
                kimLinhKhi += item.count;
            }
        }
        if (caster.IsSkillTichLuyCanUse(this, kimLinhKhi, requireLinhKhi))
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, sucTheBuff[CurrentLevelCongPhapInCombat(caster) - 1]);
        }   
        
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target,List<int> dame)
    {
       
        if (CheckIsEqualETypeSkill(caster) && CombatStatic.IsActiveWithPercent(percentX2Dame[CurrentLevelCongPhapInCombat(caster) - 1]))
        {
            
            target.AddDamageData(new List<int> { dame[0] *2 });
        }
    }
    public virtual void ExecuteParams2(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        List<int> dame = (List<int>)data[2];

        // Tìm trong danh sách trang bị của caster
        if (IsCanUse(caster))
        {
            Execute2(caster, target,dame);

        }
    }
    public override string GetDesc(int level)
    {
        return $"Mỗi tích lũy sử dụng {GetColoredText($"{requireLinhKhi}", "red")} điểm kim hệ linh khí, thu hoạch được {GetColoredText($"+{sucTheBuff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}. Tạo thành {GetTextTypeSkill()} hệ thần thông sát thương có tỉ lệ {GetColoredText($"{percentX2Dame[level - 1]}%", "red")} khiến cho sát thương lần này gấp đôi";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ExecuteParams);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams2);

    }

}
