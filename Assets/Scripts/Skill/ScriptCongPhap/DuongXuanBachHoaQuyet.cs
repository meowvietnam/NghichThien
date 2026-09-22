using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;
using System.Linq;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(DuongXuanBachHoaQuyet))]
public class DuongXuanBachHoaQuyet : CongPhap, IMemorySerializable<DuongXuanBachHoaQuyet>
{
    [SerializeField] private int require = 4;
    [SerializeField] private int[] hapThu;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int countDebuff = 0;
        foreach (var item in target.buffAndDebuffCount)
        {
            if (debuffWeights.Keys.Contains(item.Key))
            {
                countDebuff++;
            }
        }

        if (countDebuff >= require)
        {
            var dic = caster.DictHapThuLinhKhi(hapThu[CurrentLevelCongPhapInCombat(caster) - 1]);
            Debug.Log("DuongXuanBachHoaQuyet" + dic.Count);
            foreach (var item in dic)
            {
                caster.HapThuLinhKhi(item.Key, item.Value);
            }
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Lúc bắt đầu hiệp, nếu đối thủ có ít nhất {GetColoredText($"{require} loại", "red")} trạng thái tiêu cực thì bản thân hấp thụ thêm {GetColoredText($"+{hapThu[level - 1]}", "red")} linh khí";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DuongXuanBachHoaQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}