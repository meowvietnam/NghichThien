using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThieuDuongQuyet))]
public class ThieuDuongQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
          {
            12, 14 , 16,18, 20
          };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CombatStatic.IsActiveWithPercent(buffValues[CurrentLevelCongPhapInCombat(caster) - 1]))
        {
            caster.HapThuLinhKhi(1);
        }

    }

    public override string GetDesc(int level)
    {
        return $"Mỗi khi sử dụng thần thông, có {GetColoredText($"{buffValues[level - 1]}%", "red")} hấp thụ một điểm linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);

    }
}
