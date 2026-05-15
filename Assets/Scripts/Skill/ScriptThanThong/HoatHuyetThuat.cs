using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HoatHuyetThuat), menuName = "Skill/Thần Thông/" + nameof(HoatHuyetThuat))]
public class HoatHuyetThuat : AmKinhTT, IMemorySerializable<HoatHuyetThuat>
{
    [SerializeField]
    private int[] healValues;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddAmKinhTT(this, afterHiep);
    }

    public override string GetDescAmKinhTT(int level)
    {
        return $"Khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} Hp";
    }

    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.Healing(healValues[GetIndexLevelThanThong(caster)]);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HoatHuyetThuat value)
    {
        AmKinhTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}