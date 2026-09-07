using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThienMaGiaiThe), menuName = "Skill/Thần Thông/" + nameof(ThienMaGiaiThe))]
public class ThienMaGiaiThe : ThanThong, IMemorySerializable<ThienMaGiaiThe>
{
    [SerializeField] private int percent = 50;
    [SerializeField] private int[] tieuHaos = new int[] { 50, 60, 70, 80, 90 };
    [SerializeField] private int[] buffValues = new int[] { 5, 7, 9, 11, 13 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int tieuHao = tieuHaos[GetIndexLevelThanThong(caster)];
        caster.characterStatsBase.thoNguyen -= tieuHao;
        caster.totalKhiRandom += buffValues[GetIndexLevelThanThong(caster)];
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.CHARACTER_BEFORE_TAKE_DAME, ObserverEffect, -99);
    }

    private void ObserverEffect(params object[] data)
    {
        CharacterCombat targetTurn = (CharacterCombat)data[1];
        if (targetTurn == null)
            return;
        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is ThienMaGiaiThe))
        {
            targetTurn.AddDamageData(new List<int> { -(int)(percent * targetTurn.GetDameBuffForTarget() / 100f) });
        }
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        int tieuHao = tieuHaos[GetIndexLevelThanThong(caster)];
        if (caster.characterStatsBase.thoNguyen >= tieuHao &&
            !caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is ThienMaGiaiThe)) &&
            base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return
            $"Tiêu hao {GetColoredText($"-{tieuHaos[level - 1]} năm thọ nguyên", "red")}. Ở hấp thụ linh khí giai đoạn, hấp thụ linh khí {GetColoredText($"+{buffValues[level - 1]}", "red")}. Tất cả tổn thương giảm xuống {GetColoredText($"{percent}%", "red")} {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThienMaGiaiThe value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}