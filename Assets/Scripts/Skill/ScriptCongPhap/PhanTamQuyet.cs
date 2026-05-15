using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhanTamQuyet))]
public class PhanTamQuyet : CongPhap, IMemorySerializable<PhanTamQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{2, 4, 7, 12, 22};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu hiệp, đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhanTamQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}