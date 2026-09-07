using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiDuongCong))]
public class ThaiDuongCong : CongPhap, IMemorySerializable<ThaiDuongCong>
{
    [SerializeField] private int[] buffValues = new int[] { 2, 3, 4, 5, 6 };

    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        // Tìm trong danh sách trang bị của caster
        if (caster != null && IsCanUse(caster))
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
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Diem,
                count * buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Mỗi hấp thụ một điểm {GetTextLinhKhi(ELinhKhi.Hoa)} hệ linh khí thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}";
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

    public void Deserialize(IReader reader, ref ThaiDuongCong value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}