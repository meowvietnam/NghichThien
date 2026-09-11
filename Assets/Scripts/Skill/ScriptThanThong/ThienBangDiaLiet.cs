using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ThienBangDiaLiet), menuName = "Skill/Thần Thông/" + nameof(ThienBangDiaLiet))]
public class ThienBangDiaLiet : ThanThong, IMemorySerializable<ThienBangDiaLiet>
{
    [SerializeField] private int[] dames = new int[] { 24, 34, 54, 96, 192 };
    [SerializeField] protected float delayTimeEffect = 0.65f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt tran);
        dame += tran;
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, tran);
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
            $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} mỗi tiêu hao 1 tầng tăng lên 1 điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThienBangDiaLiet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}