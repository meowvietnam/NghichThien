using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(VanHonLuc))]

public class VanHonLuc : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
         {
            5,10,15,20,25
         };

    [SerializeField] private int require = 50;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Hon, out SecureInt hon);
        int count = (int)(hon / require);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Hon, count * buffValues[CurrentLevelCongPhapInCombat(caster)-1]);



    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu hiệp, mỗi có {GetColoredText($"{require}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);

    }
}
