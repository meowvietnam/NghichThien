using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ChanHuyet), menuName = "Skill/Thần Thông/" + nameof(ChanHuyet))]
public class ChanHuyet : ThongHuyet, IMemorySerializable<ChanHuyet>
{
    protected override void ExecuteAction(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKim, buff[GetIndexLevelThanThong(caster)]);
    }

    protected override string TextThuHoach()
    {
        return GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKim);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ChanHuyet value)
    {
        ThongHuyet baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}