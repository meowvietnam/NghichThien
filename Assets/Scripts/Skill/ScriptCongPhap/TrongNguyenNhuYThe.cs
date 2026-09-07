using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TrongNguyenNhuYThe))]
public class TrongNguyenNhuYThe : CongPhap, IMemorySerializable<TrongNguyenNhuYThe>
{
    [SerializeField] private int[] buff;
    [SerializeField] private int[] require;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SucThe, out SecureInt sucThe);
        if (sucThe > require[CurrentLevelCongPhapInCombat(caster) - 1])
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buff[CurrentLevelCongPhapInCombat(caster) - 1]);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Kết thúc hiệp, nếu người có số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)} lớn hơn {GetColoredText($"{require[level - 1]}", "red")} thì thu hoạch được {GetColoredText($"+{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}.";
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

    public void Deserialize(IReader reader, ref TrongNguyenNhuYThe value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}