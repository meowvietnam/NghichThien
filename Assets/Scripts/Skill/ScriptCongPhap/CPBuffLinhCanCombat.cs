using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CPBuffLinhCanCombat))]
public class CPBuffLinhCanCombat : CongPhap, IMemorySerializable<CPBuffLinhCanCombat>
{
    [SerializeField]
    private int[] buffs;
    [SerializeField]
    private ELinhKhi eLinhKhi;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, {GetTextLinhKhi(eLinhKhi)} linh căn thuộc tính {GetColoredText($"+{buffs[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        RemoveCongPhap(caster);
        int buff = buffs[CurrentLevelCongPhapInCombat(caster) - 1];
        switch (eLinhKhi)
        {
            case ELinhKhi.None:
                break;
            case ELinhKhi.Kim:
                caster.characterStatsTemp.percentKim += buff;
                break;
            case ELinhKhi.Moc:
                caster.characterStatsTemp.percentMoc += buff;
                break;
            case ELinhKhi.Thuy:
                caster.characterStatsTemp.percentThuy += buff;
                break;
            case ELinhKhi.Hoa:
                caster.characterStatsTemp.percentHoa += buff;
                break;
            case ELinhKhi.Tho:
                caster.characterStatsTemp.percentTho += buff;
                break;
            case ELinhKhi.Ma:
                caster.characterStatsTemp.percentMa += buff;
                break;
            default:
                break;
        }
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref CPBuffLinhCanCombat value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}