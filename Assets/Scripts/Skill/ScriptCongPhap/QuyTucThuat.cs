using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(QuyTucThuat))]
public class QuyTucThuat : CongPhap, IMemorySerializable<QuyTucThuat>
{
    [SerializeField]
    private int[] buffValues = new int[]{6, 12, 24, 48, 96};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (target.currentTurnDameTaked <= 0)
        {
            caster.Healing(buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        }
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, nếu chưa sử dụng bất kì thần thông gây tổn thương nào thì khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";
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

    public void Deserialize(IReader reader, ref QuyTucThuat value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}