using Newtonsoft.Json;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(MaDiemQuyet))]
public class MaDiemQuyet : CongPhap, IMemorySerializable<MaDiemQuyet>
{
    [SerializeField]
    private int[] thoNguyens = new int[]{5, 10, 20, 40, 80};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        return;
    }

    public override string GetDesc(int level)
    {
        return $"Thọ nguyên {GetColoredText($"-{thoNguyens[level - 1]}", "red")} năm";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        return;
    }

    public override void EquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.thoNguyen -= thoNguyens[daiCanhGioiCongPhap - 1];
    }

    public override void UnEquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        statsCurrent.thoNguyen += thoNguyens[daiCanhGioiCongPhap - 1];
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + thoNguyens.ByteSize();
    }

    public void Deserialize(IReader reader, ref MaDiemQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.thoNguyens = reader.ReadArray<Int32>();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.WriteArray(thoNguyens);
    }
}