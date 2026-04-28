using LongDH.LongDHModules;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NietBanQuyet))]
public class NietBanQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
        1, 1 , 2,2, 3
       };
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Tìm trong danh sách trang bị của caster
        if (target!=null&&IsCanUse(target))
        {
            Execute(caster, target, data);

        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(target.characterStatsTemp.hp <=0)
        {
            RemoveCongPhap(target);
            target.SetHp(1);
            target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out SecureInt  diem);
            target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out SecureInt  hoaDiem);
            target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt  thieuDot);
            int buff = buffValues[CurrentLevelCongPhapInCombat(target) - 1];
            int heal = (diem + hoaDiem + thieuDot) * buff;
            target.Healing(heal);
        }

    }

    public override string GetDesc(int level)
    {
        return $"Lần đầu nhận sát thương chí tử, làm mới Hp về 1. Sau đó khôi phục một lượng {GetColoredText($"{buffValues[level - 1]} * X", "red")} Hp (X là tổng số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} của bản thân)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME, ExecuteParams);

    }
}
