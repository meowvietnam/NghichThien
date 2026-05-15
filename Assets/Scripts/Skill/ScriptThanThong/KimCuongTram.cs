using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KimCuongTram), menuName = "Skill/Thần Thông/" + nameof(KimCuongTram))]
public class KimCuongTram : ThanThong, BaseSkill.LienKich, IMemorySerializable<KimCuongTram>
{
    [SerializeField]
    private int[] dame;
    [SerializeField]
    private int[] hapThu;
    [SerializeField]
    private float delayTimeEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.CaculateDameByThanThong(caster, new List<int>{dame[GetIndexLevelThanThong(caster)]}, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.HapThuLinhKhi(hapThu[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dame[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} nếu trước đó dùng Thổ hệ kĩ năng thì hấp thụ {GetColoredText($"+{hapThu}", "red")} điểm linh khí";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KimCuongTram value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}