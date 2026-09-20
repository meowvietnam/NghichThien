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
    [SerializeField] private int require = 3;
    [SerializeField] private int maxCountDame = 10;
    [SerializeField] private int requireKiemKhi = 1;
    [SerializeField] private ThanThong thanThongActive;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // Kiểm tra xem caster có đủ linh khí để dùng chiêu thức ít nhất 1 lần không
        if (caster.CurrentLinhKhi >= requireKiemKhi)
        {
            int tempLinhKhi = caster.CurrentLinhKhi;
            int availableActivations = 0;
            int maxLimit = Math.Min(10, maxCountDame); // Giới hạn tối đa 10 đòn

            // Tính toán số lần kích hoạt dựa trên chi phí lũy tiến
            while (availableActivations < maxLimit)
            {
                int hitIndex = availableActivations + 1;

                // 5 đòn đầu dùng `require`, từ đòn thứ 6 trở đi tốn 10 linh khí
                int currentCost = (hitIndex <= 5) ? require : require;

                if (tempLinhKhi >= currentCost)
                {
                    tempLinhKhi -= currentCost;
                    availableActivations++;
                }
                else
                {
                    break; // Không đủ linh khí để lên đòn tiếp theo
                }
            }

            int availableActivationsKiemKhi = caster.CurrentLinhKhi / requireKiemKhi;
            List<int> dames = new List<int>();

            // Lấy Base Dame hiện tại 1 lần outside vòng lặp để tránh gọi hàm trùng lặp
            int baseDame = this.dames[GetIndexLevelThanThong(caster)];

            caster.TieuTanAllLinhKhi();

            for (int step = 0; step < availableActivations; step++)
            {
                dames.Add(baseDame);
            }

            for (int step = 0; step < availableActivationsKiemKhi; step++)
            {
                Action(caster, target, data);
            }

            // Gửi danh sách sát thương đã được tính toán
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
            $"{GetKeyWord(EKeyWord.KiemY)}. Tiêu tán toàn bộ linh khí. Mỗi {GetColoredText($"{require}", "red")} điểm linh khí sẽ gây thêm 1 lần {GetColoredText($"{dames[level - 1]}", "red")} sát thương (Tối đa 10 lần).  Mỗi {GetColoredText($"{requireKiemKhi}", "red")} điểm linh khí thì thu hoạch được 1 tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}.";
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