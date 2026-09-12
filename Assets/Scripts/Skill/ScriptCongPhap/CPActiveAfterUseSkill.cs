using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;
using LongDHModules;
public class CPActiveAfterUseSkill : CongPhap
{
    [SerializeField]
    protected int[] percentActive = new int[] { 20, 30, 40, 60, 80 };

    //[SerializeField]
    //protected BaseSkill;
    protected override bool IsCanUse(CharacterCombat caster)
    {
        return GetCongPhapMono(caster.congPhapEquip) != null && CheckIsEqualETypeSkill(caster) && CombatStatic.IsActiveWithPercent(percentActive[CurrentLevelCongPhapInCombat(caster)-1]);
    }    

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL_COMPLETE, ExecuteParams);

        
    }
    protected virtual string DescEqual()
    {
        var types = eTypeSkill; // Lấy mảng trả về từ hàm eTypeSkill()

        if (types == null || types.Length == 0)
            return "Sử dụng thần thông"; // Giá trị dự phòng nếu mảng rỗng

        // Chuyển đổi toàn bộ mảng thành danh sách chuỗi text tương ứng
        var skillTexts = types.Select(t => GetTextTypeSkill(t)).ToArray();

        string joinedText;
        if (skillTexts.Length == 1)
        {
            joinedText = skillTexts[0];
        }
        else if (skillTexts.Length == 2)
        {
            joinedText = $"{skillTexts[0]} hoặc {skillTexts[1]}";
        }
        else
        {
            // Từ 3 phần tử trở lên: Nối các phần tử đầu bằng dấu phẩy, phần tử cuối cùng dùng "hoặc"
            string subString = string.Join(", ", skillTexts.Take(skillTexts.Length - 1));
            joinedText = $"{subString} hoặc {skillTexts[^1]}"; // Sử dụng ^1 để lấy phần tử cuối (C# 8+)
        }

        return $"Sử dụng {joinedText} hệ thần thông ";
    }

    public override string GetDesc(int level)
    {
        return
            $"{DescEqual()}, có {GetColoredText($"{percentActive[level - 1]}%", "red")} tỉ lệ ";
    }
}
