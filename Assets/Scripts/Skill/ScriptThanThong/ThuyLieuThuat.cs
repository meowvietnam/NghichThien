using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThuyLieuThuat), menuName = "Skill/Thần Thông/" + nameof(ThuyLieuThuat))]
public class ThuyLieuThuat : ThanThong, IMemorySerializable<ThuyLieuThuat>
{
    [SerializeField]
    private int[] healValues;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int heal = healValues[GetIndexLevelThanThong(caster)];
        int remainHeal = heal + (int)caster.characterStatsTemp.hp - (int)caster.characterStatsTemp.hpMax;
        if (remainHeal > 0)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, remainHeal);
        }

        caster.Healing(heal);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm Hp. Nếu đã đạt tới Hp tối đa lượng dư thừa con lại sẽ chuyển hóa thành {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThuyLieuThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}