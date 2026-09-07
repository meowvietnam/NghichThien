using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(AmDuongKinh), menuName = "Skill/Thần Thông/" + nameof(AmDuongKinh))]
public class AmDuongKinh : AmKinhTT, BaseSkill.TuongSinh, IMemorySerializable<AmDuongKinh>
{
    public enum TypeAmDuongKinh
    {
        AmDuongKinh,
        PhongLinhKinh,
        AmLoiKinh
    }

    [SerializeField]
    private int[] dames = new int[]{450, 630, 1000, 1800, 3600};
    [SerializeField]
    private int[] buffValues = new int[]{20, 30, 50, 100, 200};
    [SerializeField]
    private float delayTimeEffect = 1f;
    [SerializeField]
    private TypeAmDuongKinh currentType;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddAmKinhTT(this, afterHiep);
        PlayVisualSkill(caster,target);
    }
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        if (currentType == TypeAmDuongKinh.AmDuongKinh)
        {
            ShowSkillEffectParticleBase(caster, delayTimeEffect, 5);
        }
        else if (currentType == TypeAmDuongKinh.PhongLinhKinh || currentType == TypeAmDuongKinh.AmLoiKinh)
        {
            ShowSkillEffectWorld(target, delayTimeEffect);
        }
    }
    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dames[GetIndexLevelThanThong(target)];
        caster.TakeDameByStatic(target, new List<int>{(int)dame}, false, eTypeSkill);
    }

    public void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        int hoTrao = buffValues[GetIndexLevelThanThong(caster)];
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao);
    }

    public override string GetDescAmKinhTT(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.TuongSinh)} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref AmDuongKinh value)
    {
        AmKinhTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}