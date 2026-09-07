using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(HuyenThuyChanPhap))]
public class HuyenThuyChanPhap : CongPhap, IMemorySerializable<HuyenThuyChanPhap>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 1, 2, 3, 3 };

    public void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat
            target = data.Length > 1 ? data[1] as CharacterCombat : null; // Tìm trong danh sách trang bị của caster
        if (target != null && IsCanUse(target))
        {
            Execute2(null, target, data);
        }
    }

    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int level = CurrentLevelCongPhapInCombat(target);
        if (level == 1)
        {
            if (CombatStatic.IsActiveWithPercent(50))
            {
                target.Healing(buffValues[level - 1]);
            }
        }
        else
        {
            target.Healing(buffValues[level - 1]);
        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int level = CurrentLevelCongPhapInCombat(caster);
        if (level == buffValues.Length)
        {
            int currentValue = (int)data[1];
            int remain = currentValue - (int)caster.characterStatsTemp.hpMax;
            if (remain > 0)
            {
                if (CombatStatic.IsActiveWithPercent(50))
                {
                    caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, remain);
                }
            }
        }
    }

    public override string GetDesc(int level)
    {
        Debug.Log("HuyenThuyChanPhap level " + level);
        if (level == 1)
        {
            return $"Bị tổn thương, có 50% khôi phục {GetColoredText($"{buffValues[level - 1]}", "red")} điểm Hp";
        }
        else if (level == buffValues.Length)
        {
            return
                $"Bị tổn thương, Khôi phục {GetColoredText($"{buffValues[level - 1]}", "red")} điểm Hp. Nếu đã đầy Hp thì có 50% chuyển hóa lượng hồi phục còn thừa thành hộ tráo";
        }

        return $"Bị tổn thương, Khôi phục {GetColoredText($"{buffValues[level - 1]}", "red")} điểm Hp";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME, ExecuteParams2);
        Observer.AddObserver(EventList.Combat.CHARACTER_HEALING, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref HuyenThuyChanPhap value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}