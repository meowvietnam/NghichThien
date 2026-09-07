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
    [SerializeField] private int[] dames = new int[] { 2, 5, 10, 32, 64 };
    [SerializeField] private int require = 2;
    [SerializeField] private ThanThong thanThongActive;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // Kiểm tra xem caster có đủ linh khí để dùng chiêu thức ít nhất 1 lần không
        if (caster.CurrentLinhKhi >= require)
        {
            // Tính toán số lần có thể kích hoạt dựa trên lượng linh khí hiện tại (tránh lặp vô hạn)
            int availableActivations = caster.CurrentLinhKhi / require;
            List<int> dames = new List<int>();
            // Dùng vòng lặp thay cho đệ quy để xử lý từng lần đánh và trừ linh khí tương ứng
            for (int step = 0; step < availableActivations; step++)
            {
                // Tiêu tốn linh khí cho mỗi lần kích hoạt
                for (int i = 0; i < require; i++)
                {
                    var linhKhiNguatNhien = caster.GetRandomLinhKhiConLai();
                    caster.TieuTanLinhKhi(linhKhiNguatNhien, 1);
                }

                // Thực hiện hành động của skill (gây dame, hiệu ứng, v.v.)
                Action(caster, target, data);
                dames.Add(this.dames[GetIndexLevelThanThong(caster)]);
            }

            target.CaculateDameByThanThong(caster, dames, eTypeSkill);
          
        }
    }

    private void Action(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        KiemY(caster);

        caster.effectSkillNoDestroy.TryGetValue(this, out GameObject effect);
        if (effect == null)
        {
            PlayVisualSkill(caster, target);
        }
    }

    //Done PlayVisualSkill
    public override void PlayVisualSkill(CharacterCombat caster, CharacterCombat target)
    {
        base.PlayVisualSkill(caster, target);
        caster.effectSkillNoDestroy.Add(this, ShowSkillEffectWorld(target, 0, false));
        // Nếu không đủ linh khí yêu cầu ban đầu, tiến hành gỡ hiệu ứng sau khoảng thời gian delay
        DOVirtual.DelayedCall(delayTimeEffect, () => { caster.RemoveEffectSkillNoDestroy(this); });
    }

    public override string GetDesc(int level)
    {
        return
            $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Ngẫu nhiên tiêu tán {GetColoredText($"{require}", "red")} điểm linh khí tiếp tục kích hoạt thần thông này (Dừng lại khi linh khí không đủ)";
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