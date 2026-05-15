using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KhoLinhKinh), menuName = "Skill/Thần Thông/" + nameof(KhoLinhKinh))]
public class KhoLinhKinh : AmKinhTT, IMemorySerializable<KhoLinhKinh>
{
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddAmKinhTT(this, afterHiep);
        ShowSkillEffectParticleBase(target, delayTimeEffect, 5);
    }

    public override string GetDescAmKinhTT(int level)
    {
        return $"Nhận phải {GetColoredText($"+1", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KietSuc)} {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";
    }

    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KietSuc, 1);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is KhoLinhKinh)) && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KhoLinhKinh value)
    {
        AmKinhTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}