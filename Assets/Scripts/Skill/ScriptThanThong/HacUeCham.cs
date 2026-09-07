using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HacUeCham), menuName = "Skill/Thần Thông/" + nameof(HacUeCham))]
public class HacUeCham : ThanThong, IMemorySerializable<HacUeCham>
{
    [SerializeField] private int[] dames;
    [SerializeField] private int cham = 1;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Cham, cham);
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
        return
            $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Thu hoạch được {GetColoredText($"+{cham}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cham)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HacUeCham value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}