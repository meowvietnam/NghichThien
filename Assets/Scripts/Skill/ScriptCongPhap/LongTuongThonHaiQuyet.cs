using LongDH.LongDHModules;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LongTuongThonHaiQuyet))]
public class LongTuongThonHaiQuyet : CongPhap, IMemorySerializable<LongTuongThonHaiQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{1, 2, 3, 4, 5};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, linh khí tối đa {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        RemoveCongPhap(caster);
        caster.totalKhi += buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LongTuongThonHaiQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}