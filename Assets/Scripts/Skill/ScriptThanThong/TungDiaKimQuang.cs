using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TungDiaKimQuang), menuName = "Skill/Thần Thông/" + nameof(TungDiaKimQuang))]
public class TungDiaKimQuang : ThanThong, IMemorySerializable<TungDiaKimQuang>
{
    [SerializeField]
    private int[] buff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        var linhKhiHapThu = caster.DictHapThuLinhKhi(buff[GetIndexLevelThanThong(caster)]);
        int countKhiKim = linhKhiHapThu.GetValueOrDefault(ELinhKhi.Kim);
        if (countKhiKim > 0)
        {
            caster.HapThuLinhKhi(ELinhKhi.Kim, countKhiKim);
        }

        ShowSkillEffectWorld(caster, 1f);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"ngẫu nhiên {buff[level - 1]} điểm", "red")} linh khí. Chỉ giữ lại Kim hệ linh khí";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TungDiaKimQuang value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}