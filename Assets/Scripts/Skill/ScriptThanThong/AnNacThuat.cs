using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(AnNacThuat), menuName = "Skill/Thần Thông/" + nameof(AnNacThuat))]
public class AnNacThuat : ThanThong, BaseSkill.ThanThuc, IMemorySerializable<AnNacThuat>
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
    }

    public void ExecuteThanThuc(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.AnNac, 1);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (caster.thanThongUseThisTurn.Count <= 0 && caster.GetThanThuc() > target.GetThanThuc() && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.ThanThuc)} Đồng thời, hiệp này chưa sử dụng bất kỳ thần thông nào mới có thể sử dụng, thu hoạch được một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnNac)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref AnNacThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}