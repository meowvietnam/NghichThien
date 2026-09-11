using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyenMonDanKhiChanQuyet))]
public class HuyenMonDanKhiChanQuyet : CongPhap, IMemorySerializable<HuyenMonDanKhiChanQuyet>
{
    [SerializeField] private int[] buffValues = new int[] { 18, 21, 24, 27, 30 };
    [SerializeField] private int require = 30;

    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (target != null && IsCanUse(target))
        {
            Execute(caster, target, data);
        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (target.characterStatsTemp.hp < target.characterStatsTemp.hpMax * (require / 100f))
        {
            RemoveCongPhap(target);
            target.Healing(
                (int)(target.characterStatsTemp.hpMax * buffValues[CurrentLevelCongPhapInCombat(target) - 1]));
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Nếu HP bản thân thấp hơn {GetColoredText($"{require}%", "red")} thì lập tức khôi phục {GetColoredText($"{buffValues[level - 1]}%", "red")} điểm HP (Chỉ có tác dụng 1 lần trong mỗi cuộc chiến)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HuyenMonDanKhiChanQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}