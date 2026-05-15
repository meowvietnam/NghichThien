using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguKiemCuuQuyet))]
public class NguKiemCuuQuyet : CongPhap, IMemorySerializable<NguKiemCuuQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{1, 2, 4, 8, 12};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThong = (ThanThong)data[2];
        if (thanThong is NguKiemThuat)
        {
            caster.AddDamageData(new List<int>{buffValues[CurrentLevelCongPhapInCombat(caster) - 1]});
        }
    }

    public override string GetDesc(int level)
    {
        return $"Sử dụng thần thông [Ngự Kiếm Thuật] tổn thương {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NguKiemCuuQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}