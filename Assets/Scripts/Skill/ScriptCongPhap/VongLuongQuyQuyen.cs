using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(VongLuongQuyQuyen))]
public class VongLuongQuyQuyen : CPTieuHaoThoNguyen, IMemorySerializable<VongLuongQuyQuyen>
{
    [SerializeField]
    private int[] buffValues = new int[]{5, 18, 33, 48, 63};
    [SerializeField]
    private int[] debuffValues = new int[]{2, 4, 6, 8, 10};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int indexLevel = CurrentLevelCongPhapInCombat(caster) - 1;
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Anh, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThanThuong, debuffValues[indexLevel]);
        RemoveCongPhap(caster);
    }

    public override string GetDesc(int level)
    {
        return $"Bắt đầu chiến đấu, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Anh)}, đối thủ nhận phải {GetColoredText($"+{debuffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThanThuong)}. {base.GetDesc(level)}";
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

    public void Deserialize(IReader reader, ref VongLuongQuyQuyen value)
    {
        CPTieuHaoThoNguyen baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}