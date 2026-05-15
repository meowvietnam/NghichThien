using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThoiTamTrao), menuName = "Skill/Thần Thông/" + nameof(ThoiTamTrao))]
public class ThoiTamTrao : CuLinhTrao, IMemorySerializable<ThoiTamTrao>
{
    protected override List<CharacterCombat.PlaySkillData> GetListSkillUseCheck(CharacterCombat caster)
    {
        return caster.thanThongUseThisComBat.SelectMany(t => t).ToList();
    }

    protected override string GetLogicText()
    {
        return "Trong trận đấu này";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThoiTamTrao value)
    {
        CuLinhTrao baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}