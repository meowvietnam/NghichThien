using CreateCharacter;
using Network;
using Network.Extensions;
using UnityEngine;
using CongPhap = Skill.CongPhap;

[CreateAssetMenu(menuName = "Skill/Độn Thuật/" + nameof(NguKiemPhiHanh))]
public class NguKiemPhiHanh : CongPhap, IMemorySerializable<NguKiemPhiHanh>
{
    [SerializeField] private int[] buffValues;
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
        stats.TocDoBay += buffValues[level - 1];
        statsCurrent.CanFly = true;
    }
    public override void UnEquipCongPhap(CharacterData statsCurrent, int level)
    {
        var stats = statsCurrent.Stats;
        stats.TocDoBay -= buffValues[level - 1];
        statsCurrent.CanFly = false;
    }
    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + buffValues.ByteSize();
    }
    public void Deserialize(IReader reader, ref NguKiemPhiHanh value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.buffValues = reader.ReadArray<int>();
    }
    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.WriteArray(buffValues);
    }
}