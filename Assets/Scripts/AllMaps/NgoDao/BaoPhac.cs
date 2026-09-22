using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(BaoPhac), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(BaoPhac))]
public class BaoPhac : NgoDaoCombat, IMemorySerializable<BaoPhac>
{
    [SerializeField] private int buff = 4;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDes()
    {
        return $"Mỗi hiệp, số lượng linh khí hấp thụ {BaseSkill.GetColoredText($"+{buff}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.UpdateTotalKhiRandom(buff);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref BaoPhac value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}