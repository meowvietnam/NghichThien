using UnityEngine;
using Skill;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HoaVuThuat), menuName = "Skill/Thần Thông/" + nameof(HoaVuThuat))]
public class HoaVuThuat : ThanThong, BaseSkill.LienKich, IMemorySerializable<HoaVuThuat>
{
    [SerializeField] private int[] dames;
    [SerializeField] private int debuffCount = 2;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        PlayVisualSkill(caster, target);
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        for (int i = 0; i < debuffCount; i++)
        {
            target.AddBuffAndDebuffCount(BaseSkill.eDebuffs[UnityEngine.Random.Range(0, BaseSkill.eDebuffs.Count)], 1);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Tạo thành {GetColoredText($"{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)}. Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Thuy)} hệ thần thông, đối phương nhận {GetColoredText($"{debuffCount}", "red")} trạng thái tiêu cực ngẫu nhiên";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HoaVuThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}