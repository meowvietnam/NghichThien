using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KhuTran), menuName = "Skill/Thần Thông/" + nameof(KhuTran))]
public class KhuTran : ThanThong, IMemorySerializable<KhuTran>
{
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        var tranPhapSkills = caster.specialThanThongImpactAfterPlaySkill.OfType<TranPhapTT>().ToList();
        // Bây giờ bạn có thể thao tác với tranPhapSkills
        tranPhapSkills.ForEach(tp =>
        {
            tp.OnExecuteTranPhap(caster, target);
        });
        ShowSkillEffectParticleBase(caster, delayTimeEffect, 2.5f);
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (caster.specialThanThongImpactAfterPlaySkill.Any(x => x is TranPhapTT) && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Phát động 1 lần hiệu quả toàn bộ trận pháp của bản thân";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KhuTran value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}