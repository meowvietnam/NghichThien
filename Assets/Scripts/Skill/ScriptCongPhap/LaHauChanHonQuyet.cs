using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LaHauChanHonQuyet))]
public class LaHauChanHonQuyet : CongPhap, IMemorySerializable<LaHauChanHonQuyet>
{
    [SerializeField] private int[] buffValues = new int[] { 1, 2, 3, 4, 5 };
    [SerializeField] private int[] requires = new int[] { 10, 15, 20, 25, 30 };

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
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TinhHonChiLuc, count * buff);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Mỗi khi tích lũy gây ra đủ {GetColoredText($"{requires[level - 1]}", "red")} điểm tổn thương thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TinhHonChiLuc)}";
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

    public void Deserialize(IReader reader, ref LaHauChanHonQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}