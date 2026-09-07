using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LyHoaTuyetViem), menuName = "Skill/Thần Thông/" + nameof(LyHoaTuyetViem))]
public class LyHoaTuyetViem : ThanThong, IMemorySerializable<LyHoaTuyetViem>
{
    [SerializeField]
    protected int thieuDot = 2;
    [SerializeField]
    protected int hapThu = 3;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, thieuDot);
        caster.HapThuLinhKhi(ELinhKhi.Hoa, hapThu);
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
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Thu hoạch được {GetColoredText($"+{hapThu}", "red")} điểm {GetTextLinhKhi(ELinhKhi.Hoa)} hệ linh khí. Đồng thời, nhận phải {GetColoredText($"+{thieuDot}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LyHoaTuyetViem value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}