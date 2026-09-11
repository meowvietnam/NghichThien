using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(LamThuong), menuName = "Skill/Thần Thông/" + nameof(LamThuong))]
public class LamThuong : ThanThong, IMemorySerializable<LamThuong>
{
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        int linhKhiHapThu = caster.totalKhi - caster.CurrentLinhKhi;
        linhKhiHapThu = Mathf.Clamp(linhKhiHapThu, 0, int.MaxValue);
        var dic = caster.DictHapThuLinhKhi(linhKhiHapThu);
        foreach (var key in dic)
        {
            caster.HapThuLinhKhi(key.Key, key.Value);
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
        return
            $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Hấp thụ số linh khí thẳng đến số linh khí tối đa (Nên dùng khi số linh khí còn lại ít hơn số linh khí tối đa)";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref LamThuong value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}