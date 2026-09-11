using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(KiemDienNhanDiaThien))]
public class KiemDienNhanDiaThien : CongPhap, IMemorySerializable<KiemDienNhanDiaThien>
{
    [SerializeField] private int[] buffValues;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public override string GetDesc(int level)
    {
        return
            $"Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(GetTypeKiem())}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        caster.AddBuffAndDebuffCount(GetTypeKiem(), buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    protected virtual EBuffAndDeBuff GetTypeKiem()
    {
        var listKeyBuff = GetEKeywordBuffSkillHave(1);
        if (listKeyBuff.Contains(EBuffAndDeBuff.NhanKiem))
        {
            return EBuffAndDeBuff.NhanKiem;
        }

        if (listKeyBuff.Contains(EBuffAndDeBuff.DiaKiem))
        {
            return EBuffAndDeBuff.DiaKiem;
        }

        if (listKeyBuff.Contains(EBuffAndDeBuff.ThienKiem))
        {
            return EBuffAndDeBuff.ThienKiem;
        }

        return EBuffAndDeBuff.None;
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KiemDienNhanDiaThien value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}