using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using CreateCharacter;
using Network;
using Network.Extensions;
using CongPhap = Skill.CongPhap;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhiVanKinh))]
public class PhiVanKinh : CongPhap, IMemorySerializable<PhiVanKinh>
{
    [SerializeField]
    private int[] buffValues = new int[]{1, 2, 3, 4, 5};
    public override void EquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.tocDoBay += buffValues[daiCanhGioiCongPhap - 1];
    }

    public override void UnEquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.tocDoBay -= buffValues[daiCanhGioiCongPhap - 1];
    }
    public override void EquipCongPhap(CharacterData statsCurrent, int level)
    {
        var stats = statsCurrent.Stats;
        var tocDoBay = stats.ThanThuc + buffValues[level - 1];
        stats.TocDoBay = tocDoBay;
    }

    public override void UnEquipCongPhap(CharacterData statsCurrent, int level)
    {
        var stats = statsCurrent.Stats;
        var tocDoBay = stats.ThanThuc - buffValues[level - 1];
        stats.TocDoBay = tocDoBay;
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDesc(int level)
    {
        return $"Độn tốc {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + buffValues.ByteSize();
    }

    public void Deserialize(IReader reader, ref PhiVanKinh value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.buffValues = reader.ReadArray<Int32>();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.WriteArray(buffValues);
    }
}