using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(DuongDaoThuat))]
public class DuongDaoThuat : CongPhap, IMemorySerializable<DuongDaoThuat>
{
    [SerializeField]
    private int[] dameBuff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // CHỈ REMOVE KHI ĐÃ THỰC THI BUFF THÀNH CÔNG
        RemoveCongPhap(caster);
        int dame = dameBuff[CurrentLevelCongPhapInCombat(caster) - 1];
        target.AddDamageData(new List<int>{dame});
    }

    public override string GetDesc(int level)
    {
        return $"Mỗi cuộc chiến đấu lần thứ nhất tạo thành tổn thương {GetColoredText($"+{dameBuff[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DuongDaoThuat value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}