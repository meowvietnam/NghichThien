using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HuDocChu), menuName = "Skill/Thần Thông/" + nameof(HuDocChu))]
public class HuDocChu : ThanThong, IMemorySerializable<HuDocChu>
{
    [SerializeField] private int[] trungDoc;
    [SerializeField] private int[] dame;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.CaculateDameByThanThong(caster,
            new System.Collections.Generic.List<int> { dame[GetIndexLevelThanThong(caster)] }, eTypeSkill);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrungDoc, trungDoc[GetIndexLevelThanThong(caster)]);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return
            $"Tạo thành {GetColoredText($"+{dame[level - 1]}", "red")}. Đối phương nhận phải {GetColoredText($"+{trungDoc[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrungDoc)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HuDocChu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}