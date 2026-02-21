using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiDuongCong))]
public class ThaiDuongCong : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        2, 3 , 4,5, 6
      };
    public override void ExecuteParams(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];

        // Tìm trong danh sách trang bị của caster
        if (IsCanUse(caster))
        {
            Execute(caster, null, data);

        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ELinhKhi type = (ELinhKhi)data[1];
        int count = (int)data[2];

        if (count > 0 && type == ELinhKhi.Hoa)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Diem, count * buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);

        }


    }

    public override string GetDesc(int level)
    {
        return $"Mỗi hấp thụ một điểm {GetTextLinhKhi(ELinhKhi.Hoa)} hệ linh khí thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.ADJUST_LINHKHI, ExecuteParams);

    }
}
