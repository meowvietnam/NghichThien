using System.Collections.Generic;
using Skill;
using UnityEngine;
using static CharacterCombat;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThienQuanTram), menuName = "Skill/Thần Thông/" + nameof(ThienQuanTram))]
public class ThienQuanTram : ThanThong, IMemorySerializable<ThienQuanTram>
{
    [SerializeField] private int[] dames;
    [SerializeField] protected float delayTimeEffect = 0.35f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        SecureInt hoTrao = 0;
        SecureInt dame = dames[GetIndexLevelThanThong(caster)];
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out hoTrao);
        if (hoTrao > 0)
        {
            target.CaculateDameByThanThong(caster, new List<int> { dame * 2 }, eTypeSkill);
        }
        else
        {
            target.CaculateDameByThanThong(caster, new List<int> { dame }, eTypeSkill);
        }

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
            $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương, nếu đối phương có {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} thì nhận gấp đôi sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThienQuanTram value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}