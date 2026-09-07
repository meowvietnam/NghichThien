using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KyTruongQuyet), menuName = "Skill/Thần Thông/" + nameof(KyTruongQuyet))]
public class KyTruongQuyet : ThanThong, IMemorySerializable<KyTruongQuyet>
{
    [SerializeField]
    private int hapThuLinhKhi = 3;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        var dicHapThu = caster.DictHapThuLinhKhi(hapThuLinhKhi);
        foreach (var item in dicHapThu)
        {
            int countHapThu = item.Value;
            if (item.Key == ELinhKhi.Moc)
            {
                countHapThu *= 2;
            }

            caster.HapThuLinhKhi(item.Key, countHapThu);
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
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"+{hapThuLinhKhi} điểm", "red")} linh khí, nếu là {GetTextLinhKhi(ELinhKhi.Moc)} hệ linh khí thì hấp thụ gấp đôi";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KyTruongQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}