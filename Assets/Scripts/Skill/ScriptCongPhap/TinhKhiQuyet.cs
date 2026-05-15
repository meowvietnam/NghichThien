using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TinhKhiQuyet))]
public class TinhKhiQuyet : CongPhap, IMemorySerializable<TinhKhiQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{5, 6, 7, 8, 10};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        RemoveCongPhap(caster);
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp lần đầu tiên. Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)} (Mỗi cuộc chiến chỉ có tác dụng một lần)";
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

    public void Deserialize(IReader reader, ref TinhKhiQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}