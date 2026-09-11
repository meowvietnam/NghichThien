using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LietDiemQuyet))]
public class LietDiemQuyet : CongPhap, IMemorySerializable<LietDiemQuyet>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 1, 2, 2, 3 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDesc(int level)
    {
        return
            $"Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm {GetTextLinhKhi(ELinhKhi.Hoa)} hệ linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.HapThuLinhKhi(ELinhKhi.Hoa, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LietDiemQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}