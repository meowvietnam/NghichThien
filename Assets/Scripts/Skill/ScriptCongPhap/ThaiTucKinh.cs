using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThaiTucKinh))]
public class ThaiTucKinh : CongPhap, IMemorySerializable<ThaiTucKinh>
{
    [SerializeField]
    private int[] buffValues = new int[]{7, 14, 28, 58, 115};
    [SerializeField]
    private int requrie = 3;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.congPhapDynamic.Count(x => x.congPhap is ThaiTucKinh) >= requrie)
        {
            caster.Healing(buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
            RemoveCongPhap(caster);
        }
        else
        {
            caster.AddCPDynamic(GetCongPhapMono(caster.congPhapEquip));
        }
    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu hiệp thứ {GetColoredText($"{requrie}", "red")}. Khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP (Mỗi cuộc chiến tác dụng một lần)";
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

    public void Deserialize(IReader reader, ref ThaiTucKinh value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}