using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LuuQuangChanQuyet))]
public class LuuQuangChanQuyet : CongPhap
{
    [SerializeField] private int[] dameBuff;
    [SerializeField] private int noiThuong = 1;





    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // Ki?m tra h? c?a Skill ?ang dùng có kh?p v?i Công Pháp (SO) này không

        if (CheckIsEqualETypeSkill(caster))
        {
            int countCongDon = caster.congPhapDynamic.Count(x => x.congPhap is LuuQuangChanQuyet);
            RemoveCongPhap(caster);
            int dame = countCongDon * dameBuff[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddDamageData(new List<int> { dame });
        }
    }
    public virtual void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Tìm trong danh sách trang bị của caster
        if (caster!= null&&IsCanUse(caster))
        {
            Execute2(caster, target, data);

        }
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong,noiThuong);
        var congPhapMono = GetCongPhapMono(caster.congPhapEquip);
        caster.AddCPDynamic(congPhapMono);

    }
    public override string GetDesc(int level)
    {
        return $"Bắt Đầu hiệp, nhận phải {GetColoredText($"{noiThuong}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}, sử dụng thần thông {GetTextTypeSkill()} hệ gây tổn thương đồng thời phát động Ngũ Hành {GetKeyWord(EKeyWord.LienKich)} thì tăng lên {GetColoredText($"+{dameBuff[level - 1]}", "red")} sát thương (Nếu không kích hoạt hiệu ứng thì lượng sát thương có thể cộng dồn)";

    }


  
    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams2);
        Observer.AddObserver(EventList.Combat.LIEN_KICH, ExecuteParams);

    }
}
