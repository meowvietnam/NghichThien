using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(PhanMocQuyet), menuName = "Skill/Thần Thông/" + nameof(PhanMocQuyet))]
public class PhanMocQuyet : ThanThong, IMemorySerializable<PhanMocQuyet>
{
    [SerializeField]
    private int[] dameValues = new int[]{32, 45, 74, 132, 264};
    [SerializeField]
    private int[] dameValueBuffs = new int[]{2, 4, 11, 22, 44};
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int mocLinhKhi = caster.khiMoc;
        caster.TieuTanLinhKhi(ELinhKhi.Moc, mocLinhKhi);
        int dame = dameValues[GetIndexLevelThanThong(caster)];
        int dameBuff = dameValueBuffs[GetIndexLevelThanThong(caster)] * mocLinhKhi;
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame + dameBuff}, eTypeSkill);
        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương. Tiêu tán tất cả {GetTextLinhKhi(ELinhKhi.Moc)} hệ linh khí, mỗi một điểm tiêu tán tăng lên {GetColoredText($"+{dameValueBuffs[level - 1]}", "red")} điểm sát thương";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhanMocQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}