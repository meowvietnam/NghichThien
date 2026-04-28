using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(BachDeKiemQuyet))]
public class BachDeKiemQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
          {
            1, 2 , 3,4, 5
          };
   

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       if(CheckIsEqualETypeSkill(caster))
       {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);

        }

    }

    public override string GetDesc(int level)
    {
        return $"Mỗi sử dụng {GetTextTypeSkill(ETypeSkill.Kiem)} hệ thần thông thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);

    }
}
