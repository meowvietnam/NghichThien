using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(DietLinhQuyet))]
public class DietLinhQuyet : CongPhap
{

    [SerializeField]
    private int[] buffValues = new int[]
        {
            5, 10, 15,20, 25
        };

    [SerializeField]
    private int[] buffValues2 = new int[]
        {
            1, 2, 3,4, 5
        };
    [SerializeField] public static int require = 3;
    public override void EquipCongPhap(CharacterStatsSecure statsCurrent , int daiCanhGioiCongPhap)
    {

        statsCurrent.thanThuc += buffValues[daiCanhGioiCongPhap - 1];

      
    }
    public override void UnEquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {

        statsCurrent.thanThuc -= buffValues[daiCanhGioiCongPhap - 1];

    

    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        RemoveCongPhap(caster);


    }
    public virtual void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (caster != null && GetCongPhapMono(caster.congPhapEquip) != null)
        {
            Execute2(caster, target, data);
        }
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddCPDynamic(GetCongPhapMono(caster.congPhapEquip));
    }
    public override string GetDesc(int level)
    {
        return $"Thần thức {GetColoredText($"-{buffValues[level - 1]}", "red")}. Mỗi hiệp hấp thụ số lượng linh khí {GetColoredText($"+{buffValues2[level - 1]}", "red")}. Nhưng một hiệp chỉ có thể sử dụng {GetColoredText($"{require}", "red")} lần thần thông";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams2);
        caster.totalKhiRandom += buffValues2[CurrentLevelCongPhapInCombat(caster) - 1];
    }
}
