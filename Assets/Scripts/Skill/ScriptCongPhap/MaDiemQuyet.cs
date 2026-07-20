using Newtonsoft.Json;
using UnityEngine;
using System;
using CreateCharacter;
using Network;
using Network.Extensions;
using CongPhap = Skill.CongPhap;

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
        return $"Thọ nguyên {GetColoredText($"-{thoNguyens[level - 1]}", "red")} năm nhưng tốc độ tu luyện tăng mạnh";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        return;
    }

    public override void EquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        // statsCurrent.thoNguyen -= thoNguyens[daiCanhGioiCongPhap - 1];
    }

    public override void UnEquipCongPhap(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        // statsCurrent.thoNguyen += thoNguyens[daiCanhGioiCongPhap - 1];
    }
    public override void EquipCongPhap(CharacterData statsCurrent, int level)
    {
        // var stats = statsCurrent.Stats;
        // var thoNguyen = stats.ThoNguyen - thoNguyens[level - 1];
        // stats.ThoNguyen =  thoNguyen;
    }

    public override void UnEquipCongPhap(CharacterData statsCurrent, int level)
    {
        // var stats = statsCurrent.Stats;
        // var thoNguyen = stats.ThoNguyen + thoNguyens[level - 1];
        // stats.ThoNguyen =  thoNguyen;
    }

    public override void UpgradeCongPhapSuccess(CharacterData statsCurrent, int level)
    {
        var stats = statsCurrent.Stats;
        var thoNguyen = stats.ThoNguyen;
        var require = thoNguyens[level - 1];
        var newThoNguyen = thoNguyen - require;
        stats.SetThoNguyen(newThoNguyen);
    }

    public override void LearnCongPhapSuccess(CharacterData statsCurrent, int level)
    {
        var stats = statsCurrent.Stats;
        var thoNguyen = stats.ThoNguyen;
        var require = thoNguyens[level - 1];
        var newThoNguyen = thoNguyen - require;
        stats.SetThoNguyen(newThoNguyen);
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