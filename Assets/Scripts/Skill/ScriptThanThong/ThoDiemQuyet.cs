using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThoDiemQuyet), menuName = "Skill/Thần Thông/" + nameof(ThoDiemQuyet))]
public class ThoDiemQuyet : ThanThong, BaseSkill.LienKich, IMemorySerializable<ThoDiemQuyet>
{
    [SerializeField]
    protected int[] dames;
    [SerializeField]
    protected int[] thieuDots;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.CaculateDameByThanThong(caster, new List<int>{dames[GetIndexLevelThanThong(caster)]}, eTypeSkill, false);
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
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng Mộc hệ thần thông, thì đối thủ nhận phải {GetColoredText($"+{thieuDots[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, thieuDots[GetIndexLevelThanThong(caster)]);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThoDiemQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}