using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CuuLongDanLoiQuyet))]
public class CuuLongDanLoiQuyet : ThoNapCongPhap, IMemorySerializable<CuuLongDanLoiQuyet>
{
    [SerializeField]
    private int[] buffValues2 = new int[]{1, 2, 3, 4, 5};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, buffValues2[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    public override string GetDesc(int level)
    {
        return $"{base.GetDesc(level)}. Kết thúc hiệp, thu hoạch được {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.GiamTonThuong)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + buffValues2.ByteSize();
    }

    public void Deserialize(IReader reader, ref CuuLongDanLoiQuyet value)
    {
        ThoNapCongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.buffValues2 = reader.ReadArray<Int32>();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.WriteArray(buffValues2);
    }
}