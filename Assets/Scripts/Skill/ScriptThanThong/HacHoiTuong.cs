using DG.Tweening;
using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(HacHoiTuong), menuName = "Skill/Thần Thông/" + nameof(HacHoiTuong))]
public class HacHoiTuong : ThanThong, IMemorySerializable<HacHoiTuong>
{
    [SerializeField]
    private ThanThong thanThongActive;
    [SerializeField]
    private float delayTimeEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        SecureInt cham = 0;
        Sequence seq = DOTween.Sequence();
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Cham, out cham);
        for (int i = 0; i < cham; i++)
        {
            Debug.Log("HacHoiTuong");
            Observer.Notify(EventList.Combat.PLAY_SKILL, caster, target, thanThongActive, new List<LinhKhiTieuTan>());
            seq.AppendCallback(() =>
            {
                ShowSkillEffectWorld(caster, delayTimeEffect);
            });
            seq.AppendInterval(delayTimeEffect);
        }
    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        SecureInt cham = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Cham, out cham);
        return cham > 0 && base.IsCanUse(caster, target);
    }

    public override string GetDesc(int level)
    {
        return $"Liên tục phóng thích X lần thần thông [Phi Châm Thuật] (X là số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Cham)} bạn đang có)";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HacHoiTuong value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}