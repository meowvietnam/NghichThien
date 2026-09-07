using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HuyenAmKinh), menuName = "Skill/Thần Thông/" + nameof(HuyenAmKinh))]
public class HuyenAmKinh : AmKinhTT, IMemorySerializable<HuyenAmKinh>
{
    [SerializeField] private int[] buffValues = new int[] { 9, 9, 9, 10, 10 };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddAmKinhTT(this, afterHiep);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectParticleBase(caster, delayTimeEffect, 5);
    }

    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong, buffValues[GetIndexLevelThanThong(target)]);
    }

    public override string GetDescAmKinhTT(int level)
    {
        return
            $"Đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HuyenAmKinh value)
    {
        AmKinhTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}