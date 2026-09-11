using LongDH.LongDHModules;
using System.Collections.Generic;
using Skill;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThuyVuQuyet))]
public class ThuyVuQuyet : CongPhap, IMemorySerializable<ThuyVuQuyet>
{
    [SerializeField] private int[] buffLinhCan;

    public override void ExecuteParams(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        // lúc này là xem bản thân nhận dame là target có skill k nếu có thì sát thương phải nhận tăng lên
        if (GetCongPhapMono(caster.congPhapEquip) != null)
        {
            Execute(caster, target, data);
        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        SecureInt countHuyenVu = 0;
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HuyenVu, out countHuyenVu);
        if (countHuyenVu > 0 && GetCongPhapMono(caster.congPhapDynamic) == null)
        {
            var congPhapMono = GetCongPhapMono(caster.congPhapEquip);
            caster.AddCPDynamic(congPhapMono);
            caster.characterStatsTemp.percentThuy += buffLinhCan[CurrentLevelCongPhapInCombat(caster) - 1];
        }
        else if (countHuyenVu <= 0 && GetCongPhapMono(caster.congPhapDynamic) != null)
        {
            caster.characterStatsTemp.percentThuy -= buffLinhCan[CurrentLevelCongPhapInCombat(caster) - 1];
            RemoveCongPhap(caster);
        }
    }

    public override string GetDesc(int level)
    {
        return
            $"Bản thân có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HuyenVu)} thì {GetTextLinhKhi(ELinhKhi.Thuy)} linh căn thuộc tính {GetColoredText($"+{buffLinhCan[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        RemoveCongPhap(caster);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref ThuyVuQuyet value)
    {
        CongPhap baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}