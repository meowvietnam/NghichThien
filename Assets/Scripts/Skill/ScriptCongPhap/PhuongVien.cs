using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhuongVien))]
public class PhuongVien : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
         {
            3, 4 , 5,6, 7
         };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
        if (thanThong is TranPhapTT || thanThong.GetEKeywordSkillHave().Contains(EKeyWord.TranPhap))
        {
           caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
           RemoveCongPhap(caster);

        }

    }

    public override string GetDesc(int level)
    {
        return $"Mỗi cuộc chiến lần đầu sử dụng {GetKeyWord(EKeyWord.TranPhap)} thần thông thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);

    }
}
