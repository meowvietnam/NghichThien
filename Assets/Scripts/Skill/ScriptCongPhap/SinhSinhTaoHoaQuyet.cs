using LongDH.LongDHModules;
using NUnit.Framework;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(SinhSinhTaoHoaQuyet))]
public class SinhSinhTaoHoaQuyet : CongPhap, IMemorySerializable<SinhSinhTaoHoaQuyet>
{
    [SerializeField] private int[] buff;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTan = (List<LinhKhiTieuTan>)data[3];
        int count = 0;
        foreach (var item in linhKhiTieuTan)
        {
            count += item.count;
        }

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buff[CurrentLevelCongPhapInCombat(caster) - 1] * count);
    }

    public override string GetDesc(int level)
    {
        return
            $"Mỗi lần sử dụng thần thông phát động Ngũ Hành {GetKeyWord(EKeyWord.LienKich)} thì thu hoạch được {GetColoredText($"+{buff[level - 1]}*X", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} (X là số linh khí kĩ năng này tiêu hao)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.LIEN_KICH, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref SinhSinhTaoHoaQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}