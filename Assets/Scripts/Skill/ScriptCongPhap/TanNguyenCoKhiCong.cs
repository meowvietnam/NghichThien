using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TanNguyenCoKhiCong))]
public class TanNguyenCoKhiCong : CongPhap, IMemorySerializable<TanNguyenCoKhiCong>
{
    [SerializeField] private int[] percents = new int[] { 10, 11, 12, 13, 15 };
    [SerializeField] private int tuKhi = 1;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = (int)data[2];
        if (count < 0) //tiêu tán
        {
            for (int i = 0; i < Mathf.Abs(count); i++)
            {
                if (CombatStatic.IsActiveWithPercent(percents[CurrentLevelCongPhapInCombat(caster) - 1]))
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, tuKhi);
                }
            }
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Bất cứ khi nào, Mỗi tiêu tán một điểm linh khí thì có {GetColoredText($"{percents[level - 1]}%", "red")} tỉ lệ thu hoạch được {GetColoredText($"+{tuKhi}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.ADJUST_LINHKHI, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TanNguyenCoKhiCong value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}