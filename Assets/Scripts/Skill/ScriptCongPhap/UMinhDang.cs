using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;
using LongDHModules;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(UMinhDang))]
public class UMinhDang : CongPhap, IMemorySerializable<UMinhDang>
{
    private int[] debuffValues = new int[] { 5, 10, 15, 20, 30 };
    private int[] debuffValues2 = new int[] { 1, 2, 3, 4, 5 };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int indexLevel = CurrentLevelCongPhapInCombat(caster) - 1;
        for (int i = 0; i < debuffValues2[indexLevel]; i++)
        {
            target.AddBuffAndDebuffCount(BaseSkill.GetDebuffWithPercent(), 1);
        }
    }


    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);

        var CombatManager = Pool.Get<CombatManager>();
        var target = CombatManager.char1 == caster ? CombatManager.char2 : CombatManager.char1;
        int indexLevel = CurrentLevelCongPhapInCombat(caster) - 1;
        for (int i = 0; i < debuffValues[indexLevel]; i++)
        {
            target.AddBuffAndDebuffCount(BaseSkill.GetDebuffWithPercent(), 1);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Bắt đầu chiến đấu, đối thủ nhận phải {GetColoredText($"{debuffValues[level - 1]}", "red")} lần trạng thái tiêu cực ngẫu nhiên. Bắt đầu hiệp,  đối thủ nhận phải {GetColoredText($"{debuffValues2[level - 1]}", "red")} lần trạng thái tiêu cực ngẫu nhiê";
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref UMinhDang value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}