using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThatSatKiemQuyet))]
public class ThatSatKiemQuyet : CongPhap, IMemorySerializable<ThatSatKiemQuyet>
{
    [SerializeField]
    private int[] percent;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        Debug.Log("Data length: " + (data != null ? data.Length.ToString() : "null"));
        ETypeSkill[] eTypeSkills = (ETypeSkill[])data[3];
        if (eTypeSkills != null && CheckIsEqualETypeSkill(eTypeSkills))
        {
            if (CombatStatic.IsActiveWithPercent(percent[CurrentLevelCongPhapInCombat(caster) - 1]))
            {
                target.AddBuffAndDebuffCount(EBuffAndDeBuff.ChayMau, 1);
            }
        }
    }

    public virtual void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (caster != null && IsCanUse(caster))
        {
            Execute2(caster, target, data);
        }
    }

    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster))
        {
            if (CombatStatic.IsActiveWithPercent(percent[CurrentLevelCongPhapInCombat(caster) - 1]))
            {
                target.AddBuffAndDebuffCount(EBuffAndDeBuff.ChayMau, 1);
            }
        }
    }

    public override string GetDesc(int level)
    {
        return $"Khi tạo thành {GetTextTypeSkill()} hệ tổn thương, có {GetColoredText($"{percent[level - 1]}%", "red")} xác suất khiến đối thủ nhận phải 1 tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChayMau)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME_BY_STATIC, ExecuteParams);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams2);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThatSatKiemQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}