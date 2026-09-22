using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TauHuyet), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TauHuyet))]
public class TauHuyet : NgoDaoCombat, IMemorySerializable<TauHuyet>
{
    [SerializeField] private int buff = 1;

    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDes()
    {
        return $"Linh khí tối đa {BaseSkill.GetColoredText($"+{buff}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.UpdateTotalKhi(buff);

    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TauHuyet value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}