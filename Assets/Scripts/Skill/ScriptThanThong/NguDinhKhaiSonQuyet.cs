using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NguDinhKhaiSonQuyet), menuName = "Skill/Thần Thông/" + nameof(NguDinhKhaiSonQuyet))]
public class NguDinhKhaiSonQuyet : ThanThong, IMemorySerializable<NguDinhKhaiSonQuyet>
{
    [SerializeField] private int[] dames = new int[] { 30, 55, 110, 220, 440 };
    [SerializeField] protected float delayTimeEffect = 0.85f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        if (caster.CurrentLinhKhi > 10)
        {
            dame *= 2;
        }

        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
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
            $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương, nếu số lượng linh khí còn thừa lớn hơn 10 thì tổn thương gấp đôi";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NguDinhKhaiSonQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}