using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(NgungThuyQuyLuu), menuName = "Skill/Thần Thông/" + nameof(NgungThuyQuyLuu))]
public class NgungThuyQuyLuu : ThanThong, IMemorySerializable<NgungThuyQuyLuu>
{
    [SerializeField] private int[] healValues;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int heal = healValues[GetIndexLevelThanThong(caster)];
        ShowSkillEffectWorld(caster, delayTimeEffect);
        caster.Healing(heal * caster.CurrentLinhKhi);
    }

    public override string GetDesc(int level)
    {
        return
            $"Mỗi dư thừa một điểm linh khí thì khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm Hp";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref NgungThuyQuyLuu value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}