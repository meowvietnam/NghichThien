using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KhiToanTram), menuName = "Skill/Thần Thông/" + nameof(KhiToanTram))]
public class KhiToanTram : ThanThong, IMemorySerializable<KhiToanTram>
{
    [SerializeField]
    private int[] buff;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, buff[GetIndexLevelThanThong(caster)]);
        PlayVisualSkill(caster, target);
    }
    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Bản thân gỡ ra {GetColoredText($"{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KhiToanTram value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}