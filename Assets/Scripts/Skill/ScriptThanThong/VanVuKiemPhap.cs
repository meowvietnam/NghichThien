using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(VanVuKiemPhap), menuName = "Skill/Thần Thông/" + nameof(VanVuKiemPhap))]
public class VanVuKiemPhap : ThanThong, IMemorySerializable<VanVuKiemPhap>
{
    [SerializeField]
    protected int[] dames;
    [SerializeField]
    protected int addKiemKhi = 1;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        target.CaculateDameByThanThong(caster, new List<int>{dames[GetIndexLevelThanThong(caster)]}, eTypeSkill, false);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HuyenVu, out SecureInt huyenVu);
        if (huyenVu > 0)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, huyenVu * addKiemKhi);
        }

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
        return $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HuyenVu)} thì thu hoạch được {GetColoredText($"+{addKiemKhi}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref VanVuKiemPhap value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}