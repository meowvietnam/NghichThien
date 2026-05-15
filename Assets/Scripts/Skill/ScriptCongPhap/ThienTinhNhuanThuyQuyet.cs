using LongDH.LongDHModules;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThienTinhNhuanThuyQuyet))]
public class ThienTinhNhuanThuyQuyet : CongPhap, IMemorySerializable<ThienTinhNhuanThuyQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{2, 3, 5, 9, 16};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.Healing(buffValues[CurrentLevelCongPhapInCombat(caster) - 1] * caster.CurrentLinhKhi);
    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, khôi phục {GetColoredText($"{buffValues[level - 1]}*X", "red")} điểm Hp (X là tổng số linh khí)";
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

    public void Deserialize(IReader reader, ref ThienTinhNhuanThuyQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}