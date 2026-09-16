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
            // Tính toán số lần có thể kích hoạt dựa trên lượng linh khí hiện tại
            int availableActivations = caster.CurrentLinhKhi / require;
            List<int> dames = new List<int>();

            // Lấy Base Dame hiện tại 1 lần outside vòng lặp để tránh gọi hàm trùng lặp
            int baseDame = this.dames[GetIndexLevelThanThong(caster)];

            for (int step = 0; step < availableActivations; step++)
            {
                // 1. Tiêu tốn linh khí cho mỗi lần kích hoạt
                for (int i = 0; i < require; i++)
                {
                    var linhKhiNguatNhien = caster.GetRandomLinhKhiConLai();
                    caster.TieuTanLinhKhi(linhKhiNguatNhien, 1);
                }

                // 2. Thực hiện hành động của skill (hiệu ứng, animation...)
                Action(caster, target, data);

                // 3. Tính % giảm sát thương theo từng lần chém (Max 80%)
                // Lần 1 (step = 0): Giảm 0%   => Dame = 100% Base
                // Lần 2 (step = 1): Giảm 10%  => Dame = 90% Base
                // Lần 3 (step = 2): Giảm 20%  => Dame = 80% Base ... tối đa giảm 80% (giữ lại 20% dame)
                int reducePercent = Math.Min(step * 10, 80);
                int currentDame = Mathf.RoundToInt(baseDame * (100 - reducePercent) / 100f);

                dames.Add(currentDame);
            }

            // Gửi danh sách sát thương đã được giảm dần để tính toán
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
            $"{GetKeyWord(EKeyWord.KiemY)} Gây {GetColoredText($"{dames[level - 1]}", "red")} sát thương. Tự động tiêu tán {GetColoredText($"{require}", "red")} linh khí để tái kích hoạt (Sát thương giảm dần {GetColoredText("10%", "red")} mỗi lần, tối đa {GetColoredText("80%", "red")}) cho đến khi hết linh khí.";
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