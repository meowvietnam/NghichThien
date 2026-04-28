using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LaHauChanHonQuyet))]
public class LaHauChanHonQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
            1,2,3,4,5
       };
    [SerializeField]
    private int[] requires = new int[]
      {
            10,15,20,25,30
      };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = (int)data[2];
        int indexLevel = CurrentLevelCongPhapInCombat(caster) - 1;
        if (caster.IsSkillTichLuyCanUse(this, dame, requires[indexLevel]))
        {
            int buff = buffValues[indexLevel];
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TinhHonChiLuc, buff);

        }



    }

    public override string GetDesc(int level)
    {
        return $"Mỗi khi tích lũy gây ra đủ {GetColoredText($"{requires[level - 1]}", "red")} điểm tổn thương thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TinhHonChiLuc)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME, ExecuteParams);

    }
}
