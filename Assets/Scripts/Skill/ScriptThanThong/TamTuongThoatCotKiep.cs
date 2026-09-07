using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TamTuongThoatCotKiep), menuName = "Skill/Thần Thông/" + nameof(TamTuongThoatCotKiep))]
public class TamTuongThoatCotKiep : ThanThong, IMemorySerializable<TamTuongThoatCotKiep>
{
    [SerializeField]
    private int[] dames;
    [SerializeField]
    private int[] percentBuffDames;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        float percent = (float)percentBuffDames[GetIndexLevelThanThong(caster)] / 100f;
        dame += (int)(percent * caster.characterStatsTemp.hpMax);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
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
        return $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. Đồng thời gây ra cho đối phương thêm {GetColoredText($"{percentBuffDames[level - 1]}%", "red")} Hp tối đa của bản thân";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TamTuongThoatCotKiep value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}