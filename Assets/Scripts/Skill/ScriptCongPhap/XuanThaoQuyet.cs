using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(XuanThaoQuyet))]

public class XuanThaoQuyet : CongPhap
{
    [SerializeField] private int[] buffValues;



    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        RemoveCongPhap(caster);

        int buff = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        caster.Healing(buff);

    }
    public override string GetDesc(int level)
    {
        return $"Chiến đấu thắng lợi, bản thân khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";

    }



    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_COMBAT, ExecuteParams);
    }
}
