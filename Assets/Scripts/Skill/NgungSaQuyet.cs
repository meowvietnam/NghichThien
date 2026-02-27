using LongDH.LongDHModules;
using System.Collections.Generic;
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
            target.AddDamageData(new List<int> { buffValues[CurrentLevelCongPhapInCombat(caster) - 1] });
            target.TakeDame(caster);
        }


    }

    public override string GetDesc(int level)
    {
        return $"Khi sử dụng thần thông {GetColoredText($"[Hóa Cát]", "red")} thì tạo thành {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ExecuteParams);

    }
}
