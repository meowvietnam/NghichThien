using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TinhHaKiemPhap), menuName = "Skill/Thần Thông/" + nameof(TinhHaKiemPhap))]
public class TinhHaKiemPhap : ThanThong, BaseSkill.LienKich, IMemorySerializable<TinhHaKiemPhap>
{
    [SerializeField] private int[] buffValues = new int[] { 5, 7, 13, 27, 90 };
    [SerializeField] private int[] buffValues2 = new int[] { 1, 2, 3, 6, 11 };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, buffValues[GetIndexLevelThanThong(caster)]);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, buffValues2[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.KiemY)} Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKiem)}. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Kim)} hệ thần thông thì thu hoạch được {GetColoredText($"+{buffValues2[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKiem)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TinhHaKiemPhap value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}