using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(KiemThuatYeuQuyet))]
public class KiemThuatYeuQuyet : CongPhap, IMemorySerializable<KiemThuatYeuQuyet>
{
    [SerializeField]
    private int[] buffValues = new int[]{1, 1, 1, 1, 1};
    [SerializeField]
    private int[] requires = new int[]{30, 35, 40, 45, 50};
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster))
        {
            if (CombatStatic.IsActiveWithPercent(requires[CurrentLevelCongPhapInCombat(caster) - 1]))
            {
                caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);
            }
        }
    }

    public override string GetDesc(int level)
    {
        return $"Mỗi sử dụng {GetTextTypeSkill(ETypeSkill.Kiem)} hệ thần thông thì có {GetColoredText($"{requires[level - 1]}%", "red")} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref KiemThuatYeuQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}