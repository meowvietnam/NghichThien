using UnityEngine;
using System.Collections.Generic;
using Skill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LyHoaKiemPhap), menuName = "Skill/Thần Thông/" + nameof(LyHoaKiemPhap))]
public class LyHoaKiemPhap : ThanThong, BaseSkill.LienKich, IMemorySerializable<LyHoaKiemPhap>
{
    [SerializeField]
    private int[] dames = new int[]{2, 4, 8, 16, 32};
    [SerializeField]
    private int[] buffValues = new int[]{1, 1, 1, 1, 1};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        TakeDame(caster, target, dames, false);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    private void TakeDame(CharacterCombat caster, CharacterCombat target, int[] dames, bool isCongDon = false)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new List<int>{dame}, eTypeSkill, isCongDon);
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        TakeDame(caster, target, dames, false);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Moc)} hệ thần thông thì gây thêm một lần tổn thương và bản thân nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LyHoaKiemPhap value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}