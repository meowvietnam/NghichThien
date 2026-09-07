using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(PhanLinhQuyet), menuName = "Skill/Thần Thông/" + nameof(PhanLinhQuyet))]
public class PhanLinhQuyet : ThanThong, IMemorySerializable<PhanLinhQuyet>
{
    [SerializeField]
    private int tieuTan = 2;
    [SerializeField]
    private int tieuTanBuff = 2;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int countRemove = tieuTan;
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ThieuDot, out SecureInt thieuDot);
        if (thieuDot > 0)
        {
            countRemove += tieuTanBuff;
        }

        for (int i = 0; i < countRemove; i++)
        {
            var linhKhi = target.GetRandomLinhKhiConLai();
            target.TieuTanLinhKhi(linhKhi, 1);
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
        return $"Ngẫu nhiên tiêu tán {GetColoredText($"{tieuTan}", "red")} điểm linh khí của đối thủ. Nếu đối phương có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} thì tiêu tán thêm {GetColoredText($"{tieuTanBuff}", "red")} điểm linh khí của đối thủ";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref PhanLinhQuyet value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}