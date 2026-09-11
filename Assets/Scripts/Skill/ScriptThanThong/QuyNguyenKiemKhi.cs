using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(QuyNguyenKiemKhi), menuName = "Skill/Thần Thông/" + nameof(QuyNguyenKiemKhi))]
public class QuyNguyenKiemKhi : TietMach, IMemorySerializable<QuyNguyenKiemKhi>
{
    protected override EBuffAndDeBuff eBuffOrDebuff => EBuffAndDeBuff.ChayMau;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        KiemY(caster);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} " + base.GetDesc(level);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref QuyNguyenKiemKhi value)
    {
        TietMach baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}