using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThanhDangQuyet))]
public class ThanhDangQuyet : CongPhap, IMemorySerializable<ThanhDangQuyet>
{
    [SerializeField] private int[] trienNhieus;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // CHỈ REMOVE KHI ĐÃ THỰC THI BUFF THÀNH CÔNG
        RemoveCongPhap(caster);
        int trienNhieu = trienNhieus[CurrentLevelCongPhapInCombat(caster) - 1];
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieu);
    }

    public override string GetDesc(int level)
    {
        return
            $"Bắt đầu chiến đấu, đối phương nhận phải {GetColoredText($"+{trienNhieus[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}";
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

    public void Deserialize(IReader reader, ref ThanhDangQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}