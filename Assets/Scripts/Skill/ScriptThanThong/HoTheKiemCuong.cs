using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HoTheKiemCuong), menuName = "Skill/Thần Thông/" + nameof(HoTheKiemCuong))]
public class HoTheKiemCuong : ThanThong, IMemorySerializable<HoTheKiemCuong>
{
    [SerializeField]
    private int[] buffValues = new int[]{4, 4, 4, 4, 4};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, kiemKhi * buffValues[GetIndexLevelThanThong(caster)]);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, kiemKhi);
        PlayVisualSkill(caster, target);
    }
    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt kiemKhi);
        if (kiemKhi > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}, mỗi một tầng thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HoTheKiemCuong value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}