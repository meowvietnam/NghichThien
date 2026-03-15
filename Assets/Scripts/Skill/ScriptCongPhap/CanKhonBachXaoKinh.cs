using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CanKhonBachXaoKinh))]
public class CanKhonBachXaoKinh : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
         {
            3, 4 , 7,12, 20
         };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
        if(thanThong is KhuTran)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }    

    }

    public override string GetDesc(int level)
    {
        return $"Khi sử dụng thần thông [Khu Trận] thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ExecuteParams);

    }
}
