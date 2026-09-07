using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NguThuyQuyet), menuName = "Skill/Thần Thông/" + nameof(NguThuyQuyet))]
public class NguThuyQuyet : ThanThong, IMemorySerializable<NguThuyQuyet>
{
    [SerializeField]
    private int hapThuLinhKhi = 2;
    [SerializeField]
    private int[] buffValues;
    [SerializeField]
    private float delayTimeEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.HapThuLinhKhi(ELinhKhi.Thuy, hapThuLinhKhi);
        caster.characterStatsTemp.percentThuy += buffValues[GetIndexLevelThanThong(caster)];
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
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Thu hoạch được {GetColoredText($"+{hapThuLinhKhi}", "red")} điểm {GetTextTypeSkill()} hệ linh khí, {GetTextTypeSkill()} linh căn thuộc tính {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NguThuyQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}