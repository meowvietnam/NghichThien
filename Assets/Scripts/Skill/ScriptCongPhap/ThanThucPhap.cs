using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThanThucPhap))]
public class ThanThucPhap : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
        {
            1,3,6,9,12
        };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int thanThuong = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThanThuong, thanThuong);

        RemoveCongPhap(caster);


    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThanThuong)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ExecuteParams);

    }
}
