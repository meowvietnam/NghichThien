using CreateCharacter;
using Network;
using Network.Extensions;
using UnityEngine;
using CongPhap = Skill.CongPhap;

public class CPTieuHaoThoNguyen : CongPhap, IMemorySerializable<CPTieuHaoThoNguyen>
{
    [SerializeField] private int[] tieuHao = new int[]
    {
        2, 4, 6, 8, 10
    };

    public override void UpgradeCongPhapSuccess(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        base.UpgradeCongPhapSuccess(statsCurrent, daiCanhGioiCongPhap);
        statsCurrent.thoNguyen -= tieuHao[daiCanhGioiCongPhap - 1];
        statsCurrent.thoNguyen = Mathf.Max(statsCurrent.thoNguyen, 0);
    }

    public override void LearnCongPhapSuccess(CharacterStatsSecure statsCurrent, int daiCanhGioiCongPhap)
    {
        base.LearnCongPhapSuccess(statsCurrent, daiCanhGioiCongPhap);
        statsCurrent.thoNguyen -= tieuHao[daiCanhGioiCongPhap - 1];
        statsCurrent.thoNguyen = Mathf.Max(statsCurrent.thoNguyen, 0);
    }

    public override void LearnCongPhapSuccess(CharacterData statsCurrent, int level)
    {
        TieuHaoThoNguyen(statsCurrent, level, tieuHao);
    }

    public override void UpgradeCongPhapSuccess(CharacterData statsCurrent, int level)
    {
        TieuHaoThoNguyen(statsCurrent, level, tieuHao);
    }

    public override string GetDesc(int level)
    {
        return $"(Học tập hoặc đột phá tiêu hao thọ nguyên {GetColoredText($"{tieuHao[level - 1]} năm", "red")})";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + tieuHao.ByteSize();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.WriteArray(tieuHao);
    }

    public void Deserialize(IReader reader, ref CPTieuHaoThoNguyen value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.tieuHao = reader.ReadArray<int>();
    }
}