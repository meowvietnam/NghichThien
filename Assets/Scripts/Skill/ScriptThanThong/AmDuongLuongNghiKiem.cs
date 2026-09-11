using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(AmDuongLuongNghiKiem),
    menuName = "Skill/Thần Thông/" + nameof(AmDuongLuongNghiKiem))]
public class AmDuongLuongNghiKiem : ThanThong, BaseSkill.TuongSinh, IMemorySerializable<AmDuongLuongNghiKiem>
{
    [SerializeField] private int[] dames = new int[] { 2, 4, 6, 10, 24 };
    [SerializeField] protected int[] buffValues = new int[] { 1, 1, 2, 2, 3 };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public virtual void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThienKiem, out SecureInt thienKiem);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DiaKiem, out SecureInt diaKiem);
        if (thienKiem > 0 && diaKiem > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    protected virtual string GetDescTuongSinh(int level)
    {
        return
            $"thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

    protected virtual string GetDescRequire()
    {
        return
            $"Sở hữu {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThienKiem)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DiaKiem)}";
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.KiemY)} {GetDescRequire()} mới có thể sử dụng. Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.TuongSinh)} {GetDescTuongSinh(level)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref AmDuongLuongNghiKiem value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}