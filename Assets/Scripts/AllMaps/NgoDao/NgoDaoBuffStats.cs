using System;
using AllMaps.NgoDao;
using Network;
using Network.Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(NgoDaoBuffStats), menuName = "Ngộ Đạo/Ngộ Đạo Buff Stats/" + nameof(NgoDaoBuffStats))]
public class NgoDaoBuffStats : NgoDao, IMemorySerializable<NgoDaoBuffStats>
{
    [SerializeField]
    private EBuffStats currentEBuffStat;
    [SerializeField]
    private int buff;
    public enum EBuffStats
    {
        None,
        ThanThuc,
        HP,
        DonToc,
    }

    public override string GetDes()
    {
        return $"{GetTextBuff(currentEBuffStat)} {BaseSkill.GetColoredText($"+{buff}", "red")}";
    }

    private string GetTextBuff(EBuffStats ebuffStats)
    {
        switch (ebuffStats)
        {
            case EBuffStats.None:
                break;
            case EBuffStats.ThanThuc:
                return "Thần thức";
            case EBuffStats.HP:
                return "HP tối đa";
            case EBuffStats.DonToc:
                return "Độn tốc";
            default:
                break;
        }

        return "";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + currentEBuffStat.ByteSize() + buff.ByteSize();
    }

    public void Deserialize(IReader reader, ref NgoDaoBuffStats value)
    {
        NgoDao baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.currentEBuffStat = reader.Read<EBuffStats>();
        value.buff = reader.Read<Int32>();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.Write(currentEBuffStat);
        writer.Write(buff);
    }
}