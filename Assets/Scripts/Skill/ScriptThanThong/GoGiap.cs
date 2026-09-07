using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(GoGiap), menuName = "Skill/Thần Thông/" + nameof(GoGiap))]
public class GoGiap : ThanThong, IMemorySerializable<GoGiap>
{
    [SerializeField] protected float delayTimeEffect = 1f;
    [SerializeField] private int[] buffValues = new int[] { 2, 4, 6, 8, 15 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, buffValues[GetIndexLevelThanThong(caster)]);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out SecureInt hoTrao);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, hoTrao);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao);
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
            $"Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}. Bản thân gỡ ra toàn bộ số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} chuyển sang {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} số tầng tương đương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref GoGiap value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}