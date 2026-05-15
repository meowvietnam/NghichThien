using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiThuongHoaLongQuyet))]
public class ThaiThuongHoaLongQuyet : ThoNapCongPhap, IMemorySerializable<ThaiThuongHoaLongQuyet>
{
    [SerializeField]
    private int percent = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.Healing((int)(caster.characterStatsTemp.hpMax * percent / 100));
    }

    public override string GetDesc(int level)
    {
        return $"{base.GetDesc(level)}. Bắt đầu hiệp, Khôi phục {GetColoredText($"{percent}%", "red")} điểm HP tối đa";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + percent.ByteSize();
    }

    public void Deserialize(IReader reader, ref ThaiThuongHoaLongQuyet value)
    {
        ThoNapCongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.percent = reader.Read<Int32>();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.Write(percent);
    }
}