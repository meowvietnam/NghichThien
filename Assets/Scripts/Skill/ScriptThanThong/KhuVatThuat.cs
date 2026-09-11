using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KhuVatThuat), menuName = "Skill/Thần Thông/" + nameof(KhuVatThuat))]
public class KhuVatThuat : ThanThong, IMemorySerializable<KhuVatThuat>
{
    [SerializeField] private int[] dames = new int[] { 2, 3, 5, 9, 18 };
    [SerializeField] private int countAttack = 1;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        List<int> dameList = new List<int>();
        for (int i = 0; i < countAttack; i++)
        {
            dameList.Add(dames[GetIndexLevelThanThong(caster)]);
        }

        target.CaculateDameByThanThong(caster, dameList, eTypeSkill);
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
        return $"Tạo thành {GetDescCountAttack()} {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương";
    }

    private string GetDescCountAttack()
    {
        return countAttack == 1 ? " " : $"{GetColoredText($"{countAttack} lần", "red")}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KhuVatThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}