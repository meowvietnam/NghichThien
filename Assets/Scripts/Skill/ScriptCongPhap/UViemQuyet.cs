using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(UViemQuyet))]
public class UViemQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        3, 6 , 9,12, 20
      };
    public void ExecuteParams2(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];

        // Tìm trong danh sách trang bị của caster
        if (IsCanUse(target))
        {
            Execute2(caster, target, data);

        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster)) 
        {
            target.AddDamageData(new List<int> { buffValues[CurrentLevelCongPhapInCombat(caster)-1] });
        }


    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       if(CheckIsEqualETypeSkill(caster)) // sender là hỏa hệ thần thông thì giảm dame cho target
       {
            for (int i = 0; i < target.allDamages.Count; i++)
            {
                target.allDamages[i] -= buffValues[CurrentLevelCongPhapInCombat(target) - 1];
            } 
                
       }    


    }
    public override string GetDesc(int level)
    {
        return $"Nhận thần thông {GetTextLinhKhi(ELinhKhi.Hoa)} hệ tổn thương {GetColoredText($"-{buffValues[level - 1]}", "red")}, tăng lên {GetTextLinhKhi(ELinhKhi.Hoa)} hệ tổn thương {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ExecuteParams2);

    }
}
