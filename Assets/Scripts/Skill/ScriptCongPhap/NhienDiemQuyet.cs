using LongDH.LongDHModules;
using NUnit.Framework;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NhienDiemQuyet))]
public class NhienDiemQuyet : CongPhap, IMemorySerializable<NhienDiemQuyet>
{
    [SerializeField] private int[] buffValues = new int[] { 2, 3, 4, 6, 9 };
    [SerializeField] private int[] requires = new int[] { 10, 20, 30, 40, 60 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = (int)data[2];
        int indexLevel = CurrentLevelCongPhapInCombat(caster) - 1;
        if (caster.IsSkillTichLuyCanUse(this, dame, requires[indexLevel]))
        {
            // 1. Tìm chính xác cái Key đang nằm trong Dictionary dựa trên điều kiện của bạn
            // gọi hàm IsSkillTichLuyCanUse là nó đã trừ 1 lần nên + 1 lần đầu
            int count = 1;
            while (caster.IsSkillTichLuyCanUse(this, 0, requires[indexLevel]) == true)
            {
                count += 1;
            }

            int buff = buffValues[indexLevel];
            target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, count * buff);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Mỗi khi tích lũy gây ra đủ {GetColoredText($"{requires[level - 1]}", "red")} điểm tổn thương thì đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
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

    public void Deserialize(IReader reader, ref NhienDiemQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}