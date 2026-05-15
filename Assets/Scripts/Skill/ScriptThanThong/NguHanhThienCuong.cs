using System.Collections.Generic;
using UnityEngine;
using static CharacterCombat;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NguHanhThienCuong), menuName = "Skill/Thần Thông/" + nameof(NguHanhThienCuong))]
public class NguHanhThienCuong : KhuVatThuat, IMemorySerializable<NguHanhThienCuong>
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        base.Execute(caster, target, data);
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

    public void Deserialize(IReader reader, ref NguHanhThienCuong value)
    {
        KhuVatThuat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}