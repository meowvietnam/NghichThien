using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TuTranQuyet))]

public class TuTranQuyet : CongPhap
{

    [SerializeField]
    private int[] buffValues = new int[]
    {
        1, 2 , 3,4, 8
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ELinhKhi linhKhi = (ELinhKhi)data[1];
        int count = (int)data[2];
        if(count<0 && linhKhi == ELinhKhi.Tho)// bé là tiêu tán
        {
            int countAbs = Mathf.Abs(count);
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, countAbs * buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }    



    }

    public override string GetDesc(int level)
    {
        return $"Khi ngươi tiêu tán một điểm {GetTextLinhKhi(ELinhKhi.Tho)} hệ linh khí thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.ADJUST_LINHKHI, ExecuteParams);

    }
}
