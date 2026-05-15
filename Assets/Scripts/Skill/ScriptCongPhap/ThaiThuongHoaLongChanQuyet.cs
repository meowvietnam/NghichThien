using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiThuongHoaLongChanQuyet))]
public class ThaiThuongHoaLongChanQuyet : ThoNapCongPhap, IMemorySerializable<ThaiThuongHoaLongChanQuyet>
{
    [SerializeField]
    private int[] buffValues2 = new int[]{1, 1, 1, 2, 2};
    [SerializeField]
    private int[] maxs = new int[]{2, 4, 6, 8, 10};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = caster.congPhapDynamic.Count(x => x.congPhap is ThaiThuongHoaLongChanQuyet);
        int indexLevel = CurrentLevelCongPhapInCombat(caster) - 1;
        if (count < maxs[indexLevel])
        {
            caster.AddCPDynamic(GetCongPhapMono(caster.congPhapEquip), buffValues2[indexLevel]);
        }

        int newCount = caster.congPhapDynamic.Count(x => x.congPhap is ThaiThuongHoaLongChanQuyet);
        caster.HapThuLinhKhi(newCount - 1);
    }

    public override string GetDesc(int level)
    {
        return $"{GetDesc(level)}. Kết thúc hiệp, hấp thụ {GetColoredText($"{buffValues2[level - 1]}*X", "red")} điểm linh khí (X là số lượt hiện tại và giới hạn tối đa là {GetColoredText($"{maxs[level - 1]}", "red")})";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + buffValues2.ByteSize() + maxs.ByteSize();
    }

    public void Deserialize(IReader reader, ref ThaiThuongHoaLongChanQuyet value)
    {
        ThoNapCongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.buffValues2 = reader.ReadArray<Int32>();
        value.maxs = reader.ReadArray<Int32>();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.WriteArray(buffValues2);
        writer.WriteArray(maxs);
    }
}