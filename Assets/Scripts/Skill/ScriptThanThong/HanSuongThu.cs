using UnityEngine;
using System.Collections.Generic;
using Skill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HanSuongThu), menuName = "Skill/Thần Thông/" + nameof(HanSuongThu))]
public class HanSuongThu : ThanThong, IMemorySerializable<HanSuongThu>
{
    [SerializeField]
    private int[] dames;
    [SerializeField]
    private int suongGia = 1;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new List<int>{dame}, eTypeSkill);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, suongGia);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Đồng thời, đối thủ nhận phải {GetColoredText($"+{suongGia}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HanSuongThu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}