using DG.Tweening;
using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(CuuChuyenDuThanKiem), menuName = "Skill/Thần Thông/" + nameof(CuuChuyenDuThanKiem))]
public class CuuChuyenDuThanKiem : ThanThong, IMemorySerializable<CuuChuyenDuThanKiem>
{
    [SerializeField]
    private int[] dames = new int[]{2, 5, 10, 32, 64};
    [SerializeField]
    private int require = 2;
    [SerializeField]
    private ThanThong thanThongActive;
    [SerializeField]
    private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // Thực hiện Action lần đầu (nếu đây là logic mặc định của bạn)
        Action(caster, target, data);
        if (caster.CurrentLinhKhi >= require)
        {
            for (int i = 0; i < require; i++)
            {
                var linhKhiNguatNhien = caster.GetRandomLinhKhiConLai();
                caster.TieuTanLinhKhi(linhKhiNguatNhien, 1);
            }

            Observer.Notify(EventList.Combat.PLAY_SKILL, caster, target, thanThongActive, new List<LinhKhiTieuTan>());
        }
        else
        {
            DOVirtual.DelayedCall(delayTimeEffect, () =>
            {
                caster.RemoveEffectSkillNoDestroy(this);
            });
        }
    }

    private void Action(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int>{dame}, eTypeSkill);
        caster.effectSkillNoDestroy.TryGetValue(this, out GameObject effect);
        if (effect == null)
        {
            caster.effectSkillNoDestroy.Add(this, ShowSkillEffectWorld(target, 0, false));
        }
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Ngẫu nhiên tiêu tán {GetColoredText($"{require}", "red")} điểm linh khí tiếp tục kích hoạt thần thông này (Dừng lại khi linh khí không đủ)";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref CuuChuyenDuThanKiem value)
    {
        ThanThong baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}