using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThuyLongDan), menuName = "Skill/Thần Thông/" + nameof(ThuyLongDan))]
public class ThuyLongDan : ThanThong, BaseSkill.LienKich, IMemorySerializable<ThuyLongDan>
{
    [SerializeField] private int[] dame;
    [SerializeField] private int[] healValues;
    [SerializeField] private float delayTimeEffect = 0.5f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.CaculateDameByThanThong(caster, new List<int> { dame[GetIndexLevelThanThong(caster)] }, eTypeSkill);
        // ShowSkillEffectNoTarget(delayTimeEffect);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.Healing(healValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return
            $"Tạo thành {GetColoredText($"{dame[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Kim)} hệ kĩ năng thì khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm HP";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThuyLongDan value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}