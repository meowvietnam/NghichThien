using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(SucLinhQuyet))]
public class SucLinhQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
        {
            2, 3 , 4,5, 6
        };

    [SerializeField] private int require = 3;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(caster.thanThongUseThisTurn.Count<= require)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }    

    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, nếu số lần sử dụng thần thông chưa vượt quá {GetColoredText($"{require} lần", "red")} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ExecuteParams);

    }
}
