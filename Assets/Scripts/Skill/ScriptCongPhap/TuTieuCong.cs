using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TuTieuCong))]
public class TuTieuCong : CongPhap, IMemorySerializable<TuTieuCong>
{
    [SerializeField]
    private int[] buffValues = new int[]{6, 14, 22, 30, 38};
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (target != null && IsCanUse(target))
        {
            Execute(caster, target, data); // target là người nhận dame
        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CombatStatic.IsActiveWithPercent(buffValues[CurrentLevelCongPhapInCombat(target) - 1]))
        {
            caster.HapThuLinhKhi(1);
        }
    }

    public override string GetDesc(int level)
    {
        return $"Mỗi lần nhận tổn thương, có {GetColoredText($"{buffValues[level - 1]}%", "red")} hấp thụ một điểm linh khí";
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

    public void Deserialize(IReader reader, ref TuTieuCong value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}