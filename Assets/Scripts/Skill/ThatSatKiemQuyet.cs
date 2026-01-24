using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThatSatKiemQuyet))]
public class ThatSatKiemQuyet : CongPhap
{
    [SerializeField] private int[] percent;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


        if (CheckIsEqualETypeSkill(caster))
        {
            if (IsActiveWithPercent(percent[CurrentLevelCongPhapInCombat(caster)-1]))
            {
                target.AddBuffAndDebuffCount(EBuffAndDeBuff.ChayMau, 1);
            }    
        }


    }
 
    public override string GetDesc(int level)
    {
        return $"Khi tạo thành {GetTextTypeSkill()} hệ tổn thương, có {GetColoredText($"{percent[level - 1]}%","red")} xác suất khiến đối thủ nhận phải 1 tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChayMau)}";
    }


    public override void Init()
    {
        InitPosCallExecute();
    }

    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME, ExecuteParams);

    }
}
