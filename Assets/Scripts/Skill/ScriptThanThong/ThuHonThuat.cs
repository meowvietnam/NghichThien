using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThuHonThuat), menuName = "Skill/Thần Thông/" + nameof(ThuHonThuat))]
public class ThuHonThuat : ThanThong, IMemorySerializable<ThuHonThuat>
{
    [SerializeField] private int[] dames = new int[] { 9, 12, 20, 36, 73 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
    }

    public override string GetDesc(int level)
    {
        return
            $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương.Sử dụng thần thông này không giải trừ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnNac)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThuHonThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}