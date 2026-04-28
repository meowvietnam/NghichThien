using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguPhuDoanNguyenQuyet))]
public class NguPhuDoanNguyenQuyet : CongPhap
{
    [SerializeField]
    private int[] percents = new int[]
       {
            10, 13 , 16,19, 22
       };
    
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CombatStatic.IsActiveWithPercent(percents[CurrentLevelCongPhapInCombat(caster) - 1]))
        {
            target.AddDamageData(new List<int> { target.allDamages[0] });
        }

    }

    public override string GetDesc(int level)
    {
        return $"Có {GetColoredText($"{percents[level-1]}%", "red")} tỉ lệ tạo thành tổn thương gấp đôi";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ExecuteParams);

    }
}
