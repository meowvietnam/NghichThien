using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(DaiDeLuanHoaKinh))]
public class DaiDeLuanHoaKinh : CongPhap, IMemorySerializable<DaiDeLuanHoaKinh>
{
    [SerializeField]
    private int[] buffValues = new int[]{20, 40, 60, 80, 100};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int count = (int)data[2];
        if (count < 0) // tức là mất đi
        {
            for (int i = 0; i < Mathf.Abs(count); i++)
            {
                var debuffKeys = caster.buffAndDebuffCount.Keys.Where(key => BaseSkill.eDebuffs.Contains(key)).ToList();
                // 2. Kiểm tra xem có debuff nào để xóa không
                if (debuffKeys.Count > 0)
                {
                    if (CombatStatic.IsActiveWithPercent(buffValues[CurrentLevelCongPhapInCombat(caster) - 1]))
                    {
                        // 3. Chọn ngẫu nhiên một key trong danh sách debuff đã lọc
                        var randomDebuffKey = debuffKeys[CombatStatic.IndexRandom(0, debuffKeys.Count)];
                        // 4. Thực hiện xóa 1 Count
                        caster.RemoveBuffAndDebuffCount(randomDebuffKey, 1);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    public override string GetDesc(int level)
    {
        return $"Mỗi mất đi một điểm linh khí, có {GetColoredText($"{buffValues[level - 1]}%", "red")} gỡ ra một tầng trạng thái tiêu cực";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.ADJUST_LINHKHI, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref DaiDeLuanHoaKinh value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}