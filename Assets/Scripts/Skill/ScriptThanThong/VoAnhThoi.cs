using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(VoAnhThoi), menuName = "Skill/Thần Thông/" + nameof(VoAnhThoi))]
public class VoAnhThoi : ThanThong, IMemorySerializable<VoAnhThoi>
{
    [SerializeField] private int[] dames = new int[] { 3, 5, 8, 15, 30 };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)] + caster.GetToDoBay();
        if (caster.GetToDoBay() > target.GetToDoBay())
        {
            dame *= 2;
        }

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
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
            $"Tạo thành tổn thương tương đương với chỉ số độn tốc {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Nếu độn tốc cao hơn đối thủ thì tổn thương gấp đôi";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref VoAnhThoi value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}