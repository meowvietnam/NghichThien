using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(DaiNguHanhCamTienThu),
    menuName = "Skill/Thần Thông/" + nameof(DaiNguHanhCamTienThu))]
public class DaiNguHanhCamTienThu : ThanThong, IMemorySerializable<DaiNguHanhCamTienThu>
{
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        target.CaculateDameByThanThong(caster,
            new System.Collections.Generic.List<int> { (int)target.allDameTaked / 2 }, eTypeSkill);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Tạo thành {GetColoredText($"một nửa", "red")} của tổng lượng sát thương từ thần thông mà đối thủ đã phải chịu trong trận chiến này";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DaiNguHanhCamTienThu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}