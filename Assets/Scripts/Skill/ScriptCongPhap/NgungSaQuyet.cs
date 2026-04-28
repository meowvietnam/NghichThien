using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NgungSaQuyet))]

public class NgungSaQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
         {
            5, 10 , 20,40, 80
         };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
       
        if (thanThong is HoaCat)
        {
            int dame = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
            target.TakeDameByStatic(caster, new System.Collections.Generic.List<int> { dame }, false, eTypeSkill);
        }


    }

    public override string GetDesc(int level)
    {
        return $"Khi sử dụng thần thông {GetColoredText($"[Hóa Cát]", "red")} thì tạo thành {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);

    }
}
