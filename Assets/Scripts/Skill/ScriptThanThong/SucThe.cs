using System.Diagnostics;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(SucThe), menuName = "Skill/Thần Thông/" + nameof(SucThe))]
public class SucThe : ThanThong, IMemorySerializable<SucThe>
{
    [SerializeField]
    private int[] sucThe;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.SucThe, sucThe[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, 1f);
    }

    public override string GetDesc(int level)
    {
        return $"Bản thân thu hoạch được {GetColoredText($"+{sucThe[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SucThe)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref SucThe value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}