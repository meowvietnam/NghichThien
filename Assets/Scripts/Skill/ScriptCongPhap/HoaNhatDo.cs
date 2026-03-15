using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HoaNhatDo))]
public class HoaNhatDo : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
            1,2,3,4,5
       };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = caster.specialThanThongImpactAfterPlaySkill.Count(thanThong => thanThong is TranPhapTT || thanThong.GetEKeywordSkillHave().Contains(EKeyWord.TranPhap));
        if(count == 1)
        {
            caster.HapThuLinhKhi(buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }
       

    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, Bản thân còn lại duy nhất một {GetKeyWord(EKeyWord.TranPhap)} thì hấp thụ {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ExecuteParams);

    }
}
