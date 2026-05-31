using UnityEngine;
using System;
using CreateCharacter;
using Network;
using Network.Extensions;
using CongPhap = Skill.CongPhap;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThoNapCongPhap))]
public class ThoNapCongPhap : CongPhap, IMemorySerializable<ThoNapCongPhap>
{
    [SerializeField]
    private int[] buffValues;
    public override void EquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.hpMax += buffValues[daiCanhGioiCongPhap - 1];
    }

    public override void UnEquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.hpMax -= buffValues[daiCanhGioiCongPhap - 1];
        if (statsCurrent.hp >= statsCurrent.hpMax)
        {
            statsCurrent.hp -= buffValues[daiCanhGioiCongPhap - 1];
        }
    }
    public override void EquipCongPhap(CharacterData statsCurrent, int level)
    {
        var stats = statsCurrent.Stats;
        var sinhMenh = stats.SinhMenh + buffValues[level - 1];
        stats.SinhMenh = sinhMenh;
    }

    public override void UnEquipCongPhap(CharacterData statsCurrent, int level)
    {
        var stats = statsCurrent.Stats;
        var sinhMenh = stats.SinhMenh - buffValues[level - 1];
        stats.SinhMenh = sinhMenh;
        if (stats.Hp > sinhMenh)
        {
            stats.Hp = sinhMenh;
        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        return;
    }

    public override string GetDesc(int level)
    {
        return $"HP tối đa {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        return;
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + buffValues.ByteSize();
    }

    public void Deserialize(IReader reader, ref ThoNapCongPhap value)
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