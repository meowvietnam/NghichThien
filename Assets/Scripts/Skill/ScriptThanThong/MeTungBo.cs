using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(MeTungBo), menuName = "Skill/Thần Thông/" + nameof(MeTungBo))]
public class MeTungBo : LaYenBo, IMemorySerializable<MeTungBo>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 1, 2, 2, 3 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TangToc, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return base.GetDesc(level) +
               $". Mỗi lần thi triển, trong cuộc chiến này độn tốc {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref MeTungBo value)
    {
        LaYenBo baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}