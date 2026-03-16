using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(VongUuQuyet))]
public class VongUuQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
          {
            1, 2 , 3,4, 5
          };
    [SerializeField]
    private int[] percent = new int[]
          {
            50, 50 , 50,50, 50
          };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(caster.CurrentLinhKhi<=0)
        {
            if (CombatStatic.IsActiveWithPercent(percent[CurrentLevelCongPhapInCombat(caster) - 1]))
            {
                caster.HapThuLinhKhi(buffValues[CurrentLevelCongPhapInCombat(caster)-1]);
            }
        }    
    }
    
    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, nếu không còn linh khí thì có {GetColoredText($"{percent[level - 1]}%", "red")} hấp thụ {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);

    }
}
