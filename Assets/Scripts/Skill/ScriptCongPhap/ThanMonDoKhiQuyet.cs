using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThanMonDoKhiQuyet))]
public class ThanMonDoKhiQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
          {
            3, 5 , 9,13, 22
          };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster))
        {
            caster.Healing(buffValues[CurrentLevelCongPhapInCombat(caster)-1]);

        }

    }

    public override string GetDesc(int level)
    {
        return $"Mỗi sử dụng {GetTextTypeSkill(ETypeSkill.Than)} hệ thần thông. Khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);

    }
}
