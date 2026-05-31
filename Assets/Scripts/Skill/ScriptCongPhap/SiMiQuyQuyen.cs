using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using CreateCharacter;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(SiMiQuyQuyen))]
public class SiMiQuyQuyen : CPTieuHaoThoNguyen, IMemorySerializable<SiMiQuyQuyen>
{
    [SerializeField]
    private int[] buffValues = new int[]{5, 18, 33, 48, 63};
    [SerializeField]
    private int[] donTocs = new int[]{2, 4, 6, 8, 10};
    public override void EquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.tocDoBay += donTocs[daiCanhGioiCongPhap - 1];
    }

    public override void UnEquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.tocDoBay -= donTocs[daiCanhGioiCongPhap - 1];
    }
    public override void EquipCongPhap(CharacterData statsCurrent, int level)
    {
        var stats = statsCurrent.Stats;
        var tocDoBay = stats.ThanThuc + donTocs[level - 1];
        stats.TocDoBay = tocDoBay;
    }

    public override void UnEquipCongPhap(CharacterData statsCurrent, int level)
    {
        var stats = statsCurrent.Stats;
        var tocDoBay = stats.ThanThuc - donTocs[level - 1];
        stats.TocDoBay = tocDoBay;
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDesc(int level)
    {
        return $"Độn tốc {GetColoredText($"+{donTocs[level - 1]}", "red")}. Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)}. {base.GetDesc(level)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Anh, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + buffValues.ByteSize() + donTocs.ByteSize();
    }

    public void Deserialize(IReader reader, ref SiMiQuyQuyen value)
    {
        CPTieuHaoThoNguyen baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.buffValues = reader.ReadArray<Int32>();
        value.donTocs = reader.ReadArray<Int32>();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.WriteArray(buffValues);
        writer.WriteArray(donTocs);
    }
}