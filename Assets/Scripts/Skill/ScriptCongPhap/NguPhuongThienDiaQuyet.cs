using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguPhuongThienDiaQuyet))]
public class NguPhuongThienDiaQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
        {
            5, 9 , 18,46, 140
        };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = caster.specialThanThongImpactAfterPlaySkill.Count(x => x is TranPhapTT || x.GetEKeywordSkillHave().Contains(EKeyWord.TranPhap));
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, count * buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);


    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, mỗi có một trận pháp thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ExecuteParams);

    }
}
