using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using static CharacterCombat;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LanHoa), menuName = "Skill/Thần Thông/" + nameof(LanHoa))]
public class LanHoa : ThanThong, IMemorySerializable<LanHoa>
{
    [SerializeField]
    private int[] targetThieuDot;
    [SerializeField]
    private int[] myThieuDot;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, targetThieuDot[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, myThieuDot[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Đối thủ nhận phải {GetColoredText($"+{targetThieuDot[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}. Bản thân nhận phải {GetColoredText($"+{myThieuDot[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LanHoa value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}