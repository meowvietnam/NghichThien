using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TrongKiemQuyet))]
public class TrongKiemQuyet : CongPhap, IMemorySerializable<TrongKiemQuyet>
{
    [SerializeField]
    private int[] buff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.TieuTanLinhKhi(caster.GetRandomLinhKhiConLai(), 1);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, buff[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, ngẫu nhiên tiêu tán 1 điểm linh khí thu hoạch được {GetColoredText($"+{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}.";
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

    public void Deserialize(IReader reader, ref TrongKiemQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}