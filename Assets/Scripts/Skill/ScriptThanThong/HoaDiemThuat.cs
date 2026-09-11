using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HoaDiemThuat), menuName = "Skill/Thần Thông/" + nameof(HoaDiemThuat))]
public class HoaDiemThuat : ThanThong, IMemorySerializable<HoaDiemThuat>
{
    [SerializeField] protected int[] hoaDiems = new int[] { 1, 2, 3, 4, 5 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoaDiem, hoaDiems[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return
            $"Thu hoạch được {GetColoredText($"+{hoaDiems[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HoaDiemThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}