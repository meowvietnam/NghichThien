using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(QuyAnhTrongTrong), menuName = "Skill/Thần Thông/" + nameof(QuyAnhTrongTrong))]
public class QuyAnhTrongTrong : ThanThong, IMemorySerializable<QuyAnhTrongTrong>
{
    [SerializeField] private int[] tieuHaos = new int[] { 6, 6, 6, 6, 6 };
    [SerializeField] private int[] buffValues = new int[] { 3, 3, 3, 3, 3 };
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int require = tieuHaos[GetIndexLevelThanThong(caster)];
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnhDich, out SecureInt anhDich);
        int count = anhDich / require;
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.AnhDich, count * buffValues[GetIndexLevelThanThong(caster)]);
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
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnhDich, out SecureInt anhDich);
        int require = tieuHaos[GetIndexLevelThanThong(caster)];
        if (anhDich >= require && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Mỗi có {GetColoredText($"{tieuHaos[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)}. Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnhDich)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref QuyAnhTrongTrong value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}