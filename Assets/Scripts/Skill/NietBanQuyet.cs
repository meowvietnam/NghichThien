using LongDH.LongDHModules;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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
        int dame = (int)data[2];
        if(dame >= target.characterStatsCurrent.hp)
        {
            RemoveCongPhap(target);
            target.SetHp(1);
            target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out int diem);
            target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out int hoaDiem);
            target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out int thieuDot);
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
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME, ExecuteParams);

    }
}
