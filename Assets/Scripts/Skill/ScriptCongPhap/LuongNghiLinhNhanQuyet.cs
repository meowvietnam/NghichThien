using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LuongNghiLinhNhanQuyet))]
public class LuongNghiLinhNhanQuyet : CongPhap
{
    [SerializeField] private int[] percentActive;
    [SerializeField] private ThanThong thanThongActive;


    public override void ExecuteParams(params object[] data)
    {
        // target là ng nhận dame 
        // ktra xem ng nhận dame có công pháp này k

        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Tìm trong danh sách trang bị của caster
        if (target!=null&&IsCanUse(target) && caster != null)
        {
            Execute(caster, target, data);

        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
   {
        Debug.Log("Có vào đây");

        if (CombatStatic.IsActiveWithPercent(percentActive[CurrentLevelCongPhapInCombat(target) - 1]))
        {
            Debug.Log("PercenOk");

            Observer.Notify(EventList.Combat.PLAY_SKILL, target, caster, thanThongActive, new List<LinhKhiTieuTan>());

        }

   }
    public override string GetDesc(int level)
    {
        return $"Mỗi lần bị thương tổn, có {GetColoredText($"{percentActive[level - 1]}%", "red")} tỷ lệ phóng thích một lần {GetColoredText("[Kim Nhận]","red")} lên đối phương";

    }




    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME, ExecuteParams);
    }
}
