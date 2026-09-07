using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TieuMeTungTran), menuName = "Skill/Thần Thông/" + nameof(TieuMeTungTran))]
public class TieuMeTungTran : NeTranhTT, IMemorySerializable<TieuMeTungTran>
{
    [SerializeField] private float delayTimeEf = 2f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        if (prefabEffectSkill != null)
        {
            PlayVisualSkill(caster, target);
        }
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        ShowSkillEffectWorld(caster, delayTimeEf);
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.TranPhap)}. Bản thân có {GetColoredText($"{percentNeTranh[level - 1]}%", "red")} né tránh sát thương";
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong.Id == this.Id)) &&
            base.IsCanUse(caster, target))
        {
            return true;
        }

        return false;
    }

    protected override void NeTranhSuccess(CharacterCombat takeDameChar)
    {
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TieuMeTungTran value)
    {
        NeTranhTT baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}