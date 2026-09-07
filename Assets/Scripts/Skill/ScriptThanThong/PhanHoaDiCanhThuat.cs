using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(PhanHoaDiCanhThuat), menuName = "Skill/Thần Thông/" + nameof(PhanHoaDiCanhThuat))]
public class PhanHoaDiCanhThuat : ThanThong, IMemorySerializable<PhanHoaDiCanhThuat>
{
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt myThieuDot);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, myThieuDot);
        PlayVisualSkill(caster, target);
    }
    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt myThieuDot);
        if (myThieuDot > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Đối thủ nhận phải số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} bằng với số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} của bản thân";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhanHoaDiCanhThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}