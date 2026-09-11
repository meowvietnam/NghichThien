using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CuuChuyen))]
public class CuuChuyen : CongPhap, IMemorySerializable<CuuChuyen>
{
    [SerializeField] private int[] requires = new int[] { 10, 9, 8, 7, 5 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster))
        {
            if (caster.congPhapDynamic.Count(x => x.congPhap is CuuChuyen) >=
                requires[CurrentLevelCongPhapInCombat(caster) - 1] - 1)
            {
                RemoveCongPhap(caster);
                CombatStatic.KiemKhi(caster, target);
            }
            else
            {
                caster.AddCPDynamic(GetCongPhapMono(caster.congPhapEquip));
            }
        }
    }

    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (caster != null && GetCongPhapMono(caster.congPhapEquip) != null)
        {
            Execute(caster, target, data);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Mỗi sử dụng {GetColoredText($"{requires[level - 1]}", "red")} lần {GetTextTypeSkill(ETypeSkill.Kiem)} hệ thần thông gây tổn thương thì kích hoạt một lần {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
        RemoveCongPhap(caster);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref CuuChuyen value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}