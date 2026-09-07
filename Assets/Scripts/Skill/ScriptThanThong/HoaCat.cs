using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HoaCat), menuName = "Skill/Thần Thông/" + nameof(HoaCat))]
public class HoaCat : ThanThong, IMemorySerializable<HoaCat>
{
    [SerializeField]
    protected float delayTimeEffect = 0.85f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt tran);
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.Cat, tran);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, tran);
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
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt tran);
        if (tran > 0 && base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    public override string GetDesc(int level)
    {
        return $"Bản thân có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} mới có thể sử dụng. {GetKeyWord(EKeyWord.HauKeVoLuc)}. Gỡ ra toàn bộ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} của bản thân đối thủ nhận phải {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cat)} số tầng tương đương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HoaCat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}