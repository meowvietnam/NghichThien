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

        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        // Tìm trong danh sách trang bị của caster
        if (IsCanUse(target))
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

            Observer.Notify(EventList.GamePlay.PLAY_SKILL, target, caster, thanThongActive, new List<LinhKhiTieuTan>());

        }

   }
    public override string GetDesc(int level)
    {
        return $"Mỗi lần bị thương tổn, có {GetColoredText($"{percentActive[level - 1]}%", "red")} tỷ lệ phóng thích một lần {GetColoredText("[Kim Nhận]","red")} lên đối phương";

    }




    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);
    }
}
