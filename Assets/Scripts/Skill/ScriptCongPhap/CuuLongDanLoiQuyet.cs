using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CuuLongDanLoiQuyet))]

public class CuuLongDanLoiQuyet : ThoNapCongPhap
{
    [SerializeField]
    private int[] buffValues2 = new int[]
          {
            1, 2 , 3,4, 5
          };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, buffValues2[CurrentLevelCongPhapInCombat(caster) - 1]);


    }

    public override string GetDesc(int level)
    {
        return $"{GetDesc(level)}. Kết thúc hiệp, thu hoạch được {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.GiamTonThuong)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ExecuteParams);

    }
}
