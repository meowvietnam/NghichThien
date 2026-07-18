using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ChanLongCuuBien))]
public class ChanLongCuuBien : ThoNapCongPhap, IMemorySerializable<ChanLongCuuBien>
{
    [SerializeField]
    private int[] buffValues2 = new int[]{0, 0, 1, 1, 1};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDesc(int level)
    {
        return $"{base.GetDesc(level)}. Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanLongChiHuyet)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanLongChiHuyet, buffValues2[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + buffValues2.ByteSize();
    }

    public void Deserialize(IReader reader, ref ChanLongCuuBien value)
    {
        ThoNapCongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.buffValues2 = reader.ReadArray<Int32>();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.WriteArray(buffValues2);
    }
}