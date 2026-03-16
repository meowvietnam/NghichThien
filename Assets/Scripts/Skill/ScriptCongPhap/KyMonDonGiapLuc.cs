using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(KyMonDonGiapLuc))]
public class KyMonDonGiapLuc : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
          {
            1, 1 , 2,2, 3
          };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
        if(thanThong is TranPhapTT || thanThong.GetEKeywordSkillHave().Contains(EKeyWord.TranPhap))
        {
            caster.HapThuLinhKhi(buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }    


    }

    public override string GetDesc(int level)
    {
        return $"Mỗi một lần sử dụng thần thông loại trận pháp, hấp thụ {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);

    }
}
