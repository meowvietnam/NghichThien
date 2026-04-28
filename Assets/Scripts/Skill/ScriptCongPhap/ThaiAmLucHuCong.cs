using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiAmLucHuCong))]
public class ThaiAmLucHuCong : CongPhap
{
    [SerializeField] private int[] buff;
    [SerializeField] private int buff2;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, buff[CurrentLevelCongPhapInCombat(caster) - 1]);


    }
    public  void ExecuteParams2(params object[] data)
    {
        // target là ng nh?n dame 
        // ktra xem ng nh?n dame có công pháp này k

        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Tìm trong danh sách trang b? c?a caster
        if (caster != null && IsCanUse(caster))
        {
            Execute2(caster, target, data);

        }
    }
    public  void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, buff2);

    }
    public override string GetDesc(int level)
    {
        return $"Bắt đầu hiệp, địch nhân nhận phải {GetColoredText($"+{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}. Mỗi lần sử dụng thần thông gây tổn thương, khiến đối phương nhận phải {GetColoredText($"+{buff2}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}";
    }


   
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams2);

    }
}
