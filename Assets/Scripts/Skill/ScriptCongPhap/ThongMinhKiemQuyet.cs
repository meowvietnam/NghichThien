using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThongMinhKiemQuyet))]

public class ThongMinhKiemQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
          {
            3, 4 , 7,12, 24
          };
    [SerializeField]
    private int[] buffValues2 = new int[]
          {
            1, 2 , 5,12, 24
          };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int indexLevel = CurrentLevelCongPhapInCombat(caster) - 1;
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemTam, buffValues[indexLevel]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buffValues2[indexLevel]);


    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu hiệp, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemTam)} và {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);

    }
}
