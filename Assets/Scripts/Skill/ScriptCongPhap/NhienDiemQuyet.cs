using LongDH.LongDHModules;
using NUnit.Framework;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NhienDiemQuyet))]
public class NhienDiemQuyet : CongPhap, IMemorySerializable<NhienDiemQuyet>
{
    [SerializeField] private int[] buffValues = new int[] { 2, 3, 4, 6, 9 };
    [SerializeField] private int[] requires = new int[] { 10, 20, 30, 40, 60 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<int> dames = (List<int>)data[2];
        foreach (int dame in dames)
        {
            if (dame >= requires[CurrentLevelCongPhapInCombat(caster) - 1])
            {
                target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot,
                    buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
            }
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Mỗi tạo thành ít nhất {GetColoredText($"{requires[level - 1]}", "red")} điểm sát thương thì đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NhienDiemQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}