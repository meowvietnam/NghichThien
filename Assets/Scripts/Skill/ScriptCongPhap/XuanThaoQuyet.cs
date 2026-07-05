using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(XuanThaoQuyet))]
public class XuanThaoQuyet : CongPhap, IMemorySerializable<XuanThaoQuyet>
{
    [SerializeField]
    private int[] buffValues;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        bool isUserWin = (bool)data[2];
        {
            RemoveCongPhap(caster);
            int buff = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
            caster.Healing(buff);
        }
    }

    public override string GetDesc(int level)
    {
        return $"Chiến đấu thắng lợi, bản thân khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref XuanThaoQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}