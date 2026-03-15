using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiAtKiemQuyet))]
public class ThaiAtKiemQuyet : CongPhap
{

    [SerializeField]
    private int[] buffValues = new int[]
         {
            19, 23 , 27,31, 35
         };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster))
        {
            target.AddDamageData(new List<int> { (int)(target.allDamages[0] * buffValues[CurrentLevelCongPhapInCombat(caster)-1]/100) });

        }

    }

    public override string GetDesc(int level)
    {
        return $"Sử dụng {GetTextTypeSkill(ETypeSkill.Kiem)} hệ thần thông gây tổn thương tăng lên {GetColoredText($"{buffValues[level - 1]}%", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ExecuteParams);

    }
}
