using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(KimCuongLuyenTheQuyet))]
public class KimCuongLuyenTheQuyet : CongPhap, IMemorySerializable<KimCuongLuyenTheQuyet>
{
    [SerializeField] private int[] hoTraoBuff;

    // private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<LinhKhiTieuTan> linhKhiTieuTan = data[2] as List<LinhKhiTieuTan>;
        int count = 0;
        foreach (var item in linhKhiTieuTan)
        {
            count += item.count;
        }

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao,
            count * hoTraoBuff[CurrentLevelCongPhapInCombat(caster) - 1]);
        //  ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return
            $"Giai đoạn tiêu tán linh khí, mỗi tiêu tán 1 điểm linh khí thu hoạch được {GetColoredText($"+{hoTraoBuff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
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

    public void Deserialize(IReader reader, ref KimCuongLuyenTheQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}