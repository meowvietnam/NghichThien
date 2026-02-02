using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HoiXuanQuyet))]

public class HoiXuanQuyet : CongPhap
{
    [SerializeField] private int[] buffValues;
    [SerializeField] private int require = 5;



    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        if(caster.khiMocUsed >= require)
        {
            caster.khiMocUsed -= require;
            int buff = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
            caster.Healing(buff);

        }


    }
    public override string GetDesc(int level)
    {
        return $"Mỗi tích lũy sử dụng {GetColoredText($"{require}", "red")} điểm {GetTextTypeSkill()} hệ linh khí, bản thân khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";

    }



    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ExecuteParams);
    }
}
