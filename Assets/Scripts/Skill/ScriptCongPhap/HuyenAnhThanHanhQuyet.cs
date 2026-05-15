using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyenAnhThanHanhQuyet))]
public class HuyenAnhThanHanhQuyet : PhiVanKinh, IMemorySerializable<HuyenAnhThanHanhQuyet>
{
    [SerializeField]
    private int[] percents = new int[]{12, 14, 16, 18, 20};
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        // target ng nhận dame là ng cần check né tránh
        if (target != null && IsCanUse(target))
        {
            Execute(caster, target, data);
        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // target ng nhận dame là ng cần check né tránh
        int index = (int)data[2];
        if (target.GetToDoBay() > caster.GetToDoBay())
        {
            if (target == null)
                return;
            if (target.allDamages[index] == -1)
                return;
            if (CombatStatic.IsActiveWithPercent(percents[CurrentLevelCongPhapInCombat(target) - 1]))
            {
                target.allDamages[index] = -1;
            }
        }
    }

    public override string GetDesc(int level)
    {
        return $"{base.GetDesc(level)}. Nếu độn tốc lớn hơn đối thủ, Có {GetColoredText($"{percents[level - 1]}%", "red")} tỉ lệ né tránh sát thương";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHECK_NE_TRANH, ExecuteParams, 1);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize + percents.ByteSize();
    }

    public void Deserialize(IReader reader, ref HuyenAnhThanHanhQuyet value)
    {
        PhiVanKinh baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
        value.percents = reader.ReadArray<Int32>();
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.WriteArray(percents);
    }
}