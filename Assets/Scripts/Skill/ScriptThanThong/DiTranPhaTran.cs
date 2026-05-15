using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(DiTranPhaTran), menuName = "Skill/Thần Thông/" + nameof(DiTranPhaTran))]
public class DiTranPhaTran : ThanThong, IMemorySerializable<DiTranPhaTran>
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        PhaTran(caster);
        PhaTran(target);
    }

    private void PhaTran(CharacterCombat target)
    {
        var tranPhapSkills = target.specialThanThongImpactAfterPlaySkill.OfType<TranPhapTT>().ToList();
        if (tranPhapSkills.Count <= 0)
            return;
        var tranPhapRandom = tranPhapSkills[CombatStatic.IndexRandom(0, tranPhapSkills.Count)];
        target.RemoveAllThanThongImpactAfterPlaySkill(tranPhapRandom);
    }

    public override string GetDesc(int level)
    {
        return $"Ngẫu nhiên phá hủy một cái trận pháp của bản thân và đối thủ";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DiTranPhaTran value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}