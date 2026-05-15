using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguDocThucTamQuyet))]
public class NguDocThucTamQuyet : CongPhap, IMemorySerializable<NguDocThucTamQuyet>
{
    [SerializeField]
    private int[] debuffValues;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int debuff = debuffValues[CurrentLevelCongPhapInCombat(caster) - 1];
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, debuff);
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, đối phương nhận phải {GetColoredText($"+{debuffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NguDocThucTamQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}