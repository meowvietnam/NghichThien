using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyetHonLuc))]

public class HuyetHonLuc : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
            6,10,16,28,48
       };
    [SerializeField]
    private int[] requires = new int[]
       {
            6,12,24,48,96
       };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int indexLevel = CurrentLevelCongPhapInCombat(caster) - 1;
        caster.TakeDameByStatic(null, new List<int> { requires[indexLevel] },false,null,false);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Hon, buffValues[indexLevel]);

    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, tiêu hao {GetColoredText($"{requires[level - 1]}", "red")} điểm HP thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);

    }
}
