using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NhatKhiPhanThanCong))]
public class NhatKhiPhanThanCong : CongPhap, IMemorySerializable<NhatKhiPhanThanCong>
{
    [SerializeField] private int[] hapThuLinhKhi;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDesc(int level)
    {
        return
            $"Linh khí tối đa là 0, mỗi hiệp hấp thụ linh khí {GetColoredText($"+{hapThuLinhKhi[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        int khi = hapThuLinhKhi[CurrentLevelCongPhapInCombat(caster) - 1];
        caster.UpdateTotalKhi(-caster.totalKhi);
        caster.UpdateTotalKhiRandom(khi);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NhatKhiPhanThanCong value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}