using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TeHuyetTrao), menuName = "Skill/Thần Thông/" + nameof(TeHuyetTrao))]
public class TeHuyetTrao : ThanThong, IMemorySerializable<TeHuyetTrao>
{
    [SerializeField]
    private int[] buffValues = new int[]{20, 30, 50, 80, 160};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int hp = buffValues[GetIndexLevelThanThong(caster)];
        caster.TakeDameByStatic(null, new System.Collections.Generic.List<int>{hp}, false, null, false);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hp);
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
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Chuyển hóa {GetColoredText($"{buffValues[level - 1]}", "red")} điểm HP của bản thân thành {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TeHuyetTrao value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}