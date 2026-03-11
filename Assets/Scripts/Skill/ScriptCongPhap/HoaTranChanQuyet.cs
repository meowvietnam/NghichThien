using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HoaTranChanQuyet))]
public class HoaTranChanQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        3, 7 , 15,30, 70
    };
   
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
        if (thanThong is HoanTran)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }
        else if (thanThong is HoaCat)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanTho, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }


    }

    public override string GetDesc(int level)
    {
        return $"Khi sử dụng thần thông {GetColoredText($"[Hoàn Trần]", "red")} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}. Khi sử dụng thần thông {GetColoredText($"[Hóa Cát]", "red")} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanTho)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ExecuteParams);

    }
}
