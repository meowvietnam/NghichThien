using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(PhaPhuTramChu), menuName = "Skill/Thần Thông/" + nameof(PhaPhuTramChu))]
public class PhaPhuTramChu : ThanThong, IMemorySerializable<PhaPhuTramChu>
{
    [SerializeField]
    protected int[] dames;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KinhDao, out SecureInt kinhDao);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChiThuy, out SecureInt chiThuy);
        int dame = dames[GetIndexLevelThanThong(caster)];
        if (kinhDao > chiThuy)
        {
            dame *= 2;
        }
        else if (chiThuy > kinhDao)
        {
            caster.Healing(dame);
        }

        target.CaculateDameByThanThong(caster, new List<int>{dame}, eTypeSkill, false);
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
        return $"{GetKeyWord(EKeyWord.KiemY)}. Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Nếu số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} lớn hơn số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} thì tổn thương gấp bội. Nếu không thì, bản thân hồi Hp bằng với sát thương gây ra";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhaPhuTramChu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}