using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyenVuThoNapThuat))]

public class HuyenVuThoNapThuat : CongPhap
{

    [SerializeField]
    private int[] buffValues = new int[]
     {
        1, 2 , 3 ,4 , 5
     };
    [SerializeField]
    private int[] percent = new int[]
     {
        10, 15 , 20 ,25 , 30
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
        int count = (int)data[2];
        if(count>0)
        {
            
            int percentFinal = count * percent[CurrentLevelCongPhapInCombat(caster) - 1];
            if(CombatStatic.IsActiveWithPercent(percentFinal))
            {
                Debug.Log("HuyenVuThoNapThuat count linh khi la " + count);
                caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HuyenVu, out int huyenVu);
                caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, huyenVu);
            }

        }

    }
    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HuyenVu)}. Mỗi hấp thụ một điểm linh khí có {GetColoredText($"{percent[level - 1]}%", "red")} tỉ lệ thu hoạch được {GetColoredText($"X tầng", "red")} {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)} (X là số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HuyenVu)} của bản thân)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HuyenVu, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        Observer.AddObserver(EventList.GamePlay.ADJUST_LINHKHI, ExecuteParams);

    }
}
