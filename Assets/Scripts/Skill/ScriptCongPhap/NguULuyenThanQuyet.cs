using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguULuyenThanQuyet))]
public class NguULuyenThanQuyet : CongPhap, IMemorySerializable<NguULuyenThanQuyet>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 2, 3, 4, 5 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.GetThanThuc() > target.GetThanThuc())
        {
            int treKhi = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, treKhi);
        }

        RemoveCongPhap(caster);
    }

    public override string GetDesc(int level)
    {
        return
            $"Bắt đầu chiến đấu, nếu thần thức lớn hơn đối thủ thì khiến đối phương nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}";
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

    public void Deserialize(IReader reader, ref NguULuyenThanQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}