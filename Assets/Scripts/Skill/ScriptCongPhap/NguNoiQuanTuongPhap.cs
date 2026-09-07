using UnityEngine;
using System;
using CreateCharacter;
using Network;
using Network.Extensions;
using CongPhap = Skill.CongPhap;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguNoiQuanTuongPhap))]
public class NguNoiQuanTuongPhap : CongPhap, IMemorySerializable<NguNoiQuanTuongPhap>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 2, 3, 4, 5 };

    public override void EquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.thanThuc += buffValues[daiCanhGioiCongPhap - 1];
    }

    public override void UnEquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.thanThuc -= buffValues[daiCanhGioiCongPhap - 1];
    }

    public override void EquipCongPhap(CharacterData statsCurrent, int level)
    {
        var stats = statsCurrent.Stats;
        var thanThuc = stats.ThanThuc + buffValues[level - 1];
        stats.ThanThuc = thanThuc;
    }

    public override void UnEquipCongPhap(CharacterData statsCurrent, int level)
    {
        var stats = statsCurrent.Stats;
        var thanThuc = stats.ThanThuc - buffValues[level - 1];
        stats.ThanThuc = thanThuc;
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDesc(int level)
    {
        return $"Thần thức {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + buffValues.ByteSize();
    }

    public void Deserialize(IReader reader, ref NguNoiQuanTuongPhap value)
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