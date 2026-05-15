using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(KySinhDanHuyetThuat), menuName = "Skill/Thần Thông/" + nameof(KySinhDanHuyetThuat))]
public class KySinhDanHuyetThuat : ThanThong, IMemorySerializable<KySinhDanHuyetThuat>
{
    [SerializeField]
    private int[] dames;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        SecureInt trienNhieu = 0;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.TrienNhieu, out trienNhieu);
        if (trienNhieu > 0)
        {
            caster.Healing(dame);
        }

        ShowSkillEffectWorld(target, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]} điểm", "red")} sát thương, nếu là {GetTextLinhKhi(ELinhKhi.Moc)} hệ linh khí thì hấp thụ gấp đôi";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KySinhDanHuyetThuat value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}