using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ChietTinhBoKhiQuyet), menuName = "Skill/Thần Thông/" + nameof(ChietTinhBoKhiQuyet))]
public class ChietTinhBoKhiQuyet : ThanThong, IMemorySerializable<ChietTinhBoKhiQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{8, 12, 16, 20, 24};
    [SerializeField]
    private float delayTimeEffect = 1f;
    [SerializeField]
    private float cameraSize = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NgungThan, out SecureInt ngungThan);
        caster.Healing(ngungThan * buffValues[GetIndexLevelThanThong(caster)]);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.NgungThan, ngungThan);
        PlayVisualSkill(caster, target);
    }
    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectParticleBase(caster, delayTimeEffect, cameraSize);
    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.NgungThan, out SecureInt ngungThan);
        if (ngungThan > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Gỡ ra toàn bộ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NgungThan)} Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NgungThan)} hồi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ChietTinhBoKhiQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}