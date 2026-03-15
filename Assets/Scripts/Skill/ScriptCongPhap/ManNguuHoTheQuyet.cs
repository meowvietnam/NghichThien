using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ManNguuHoTheQuyet))]

public class ManNguuHoTheQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
            6, 12 , 24,48, 96
       };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.characterStatsTemp.hp >= caster.characterStatsTemp.hpMax)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);

        }

    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, nếu đang ở trạng thái đầy máu thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ExecuteParams);

    }
}
