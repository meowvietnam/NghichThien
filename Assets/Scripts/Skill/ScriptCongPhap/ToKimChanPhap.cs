using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ToKimChanPhap))]
public class ToKimChanPhap : CongPhap
{
    [SerializeField] private int[] dame;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTan = data[2] as List<LinhKhiTieuTan>;
        int count = 0;
        foreach (var item in linhKhiTieuTan)
        {
            count += item.count;
        }
        Debug.Log("lmm"+count);
        target.TakeDameByStatic(caster, new System.Collections.Generic.List<int> { count * dame[CurrentLevelCongPhapInCombat(caster) - 1] }, true, eTypeSkill);
    }

    public override string GetDesc(int level)
    {
        return $"Giai đoạn tiêu tán linh khí, mỗi tiêu tán 1 điểm linh khí đối phương nhận {GetColoredText($"+{dame[level - 1]}", "red")} sát thương";
    }


  
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);
    }

}
   
