using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HoaSonKinh))]
public class HoaSonKinh : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
         {
            1, 2 , 4,6, 9
         };
    [SerializeField]
    private int[] requires = new int[]
    {
       2,4 ,8,12, 18
    };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.TakeDameByStatic(null, new List<int> { requires[CurrentLevelCongPhapInCombat(caster) - 1] },false,null,false);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Hon, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, mất đi {GetColoredText($"{requires[level - 1]}", "red")} điểm HP, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);

    }
}
