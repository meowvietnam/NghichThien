using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HoaTranQuyet))]

public class HoaTranQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        2, 3 , 4,5, 6
    };
    [SerializeField]
    private int[] dames = new int[]
    {
        2, 3 , 4,5, 6
    };
 
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
        if(thanThong is HoanTran)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }    
        else if (thanThong is HoaCat)
        {
            int dame = dames[CurrentLevelCongPhapInCombat(caster) -1];
            target.TakeDameByStatic(caster, new System.Collections.Generic.List<int> { dame }, false , eTypeSkill);
        }


    }
  
    public override string GetDesc(int level)
    {
        return $"Khi sử dụng thần thông {GetColoredText($"[Hoàn Trần]", "red")} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}. Khi sử dụng thần thông {GetColoredText($"[Hóa Cát]", "red")} thì tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm tổn thương";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);

    }
}
