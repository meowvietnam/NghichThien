using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using System;
using LongDHModules;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyenDuongBoDiemQuyet))]
public class HuyenDuongBoDiemQuyet : CongPhap, IMemorySerializable<HuyenDuongBoDiemQuyet>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 1, 2, 2, 3 };

    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = caster != Pool.Get<CombatManager>().char1
            ? Pool.Get<CombatManager>().char1
            : Pool.Get<CombatManager>().char2;
        // Tìm trong danh sách trang bị của caster
        if (caster != null && IsCanUse(caster))
        {
            Execute(caster, target, data);
        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int oldValue = (int)data[3];
        EBuffAndDeBuff type = (EBuffAndDeBuff)data[1];
        if (type == EBuffAndDeBuff.HoaDiem)
        {
            int buff = buffValues[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, buff * oldValue);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Thoát khỏi trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)}, đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}*X", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)} (X là số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)})";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.REMOVE_BUFF, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HuyenDuongBoDiemQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}