using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThaiThanhDieuPhap), menuName = "Skill/Thần Thông/" + nameof(ThaiThanhDieuPhap))]
public class ThaiThanhDieuPhap : ThanThong, IMemorySerializable<ThaiThanhDieuPhap>
{
    [SerializeField]
    private int[] buffs;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.characterStatsTemp.hpMax += buffs[GetIndexLevelThanThong(caster)];
        caster.Healing(0);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Tăng tạm thời {GetColoredText($"+{buffs[level - 1]}", "red")} Hp tối đa trong trận chiến này.";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThaiThanhDieuPhap value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}