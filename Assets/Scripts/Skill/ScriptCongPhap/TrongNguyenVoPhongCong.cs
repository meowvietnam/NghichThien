using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TrongNguyenVoPhongCong))]
public class TrongNguyenVoPhongCong : CongPhap, IMemorySerializable<TrongNguyenVoPhongCong>
{
    [SerializeField] private int[] buff;
    [SerializeField] private int require = 2;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ThanThong thanThongUse = (ThanThong)data[2];
        if (thanThongUse is SucThe && caster.khiKim >= require)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, buff[CurrentLevelCongPhapInCombat(caster) - 1]);
            caster.TieuTanLinhKhi(ELinhKhi.Kim, require);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Sử dụng thần thông {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)} thì sẽ tiêu hao thêm {GetColoredText($"{require} điểm kim", "red")} linh khí thì thu hoạch được {GetColoredText($"+{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}";
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

    public void Deserialize(IReader reader, ref TrongNguyenVoPhongCong value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}