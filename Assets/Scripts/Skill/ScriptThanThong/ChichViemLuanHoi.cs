using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(ChichViemLuanHoi), menuName = "Skill/Thần Thông/" + nameof(ChichViemLuanHoi))]
public class ChichViemLuanHoi : ThanThong, IMemorySerializable<ChichViemLuanHoi>
{
    [SerializeField]
    private int hapThu = 4;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.TieuTanAllLinhKhi();
        caster.HapThuLinhKhi(hapThu);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tiêu tán toàn bộ linh khí. Hấp thụ {GetColoredText($"+{hapThu}", "red")} điểm linh khí";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ChichViemLuanHoi value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}